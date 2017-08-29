module Web.Service

open System
open System.IO
open System.Text
open System.Text.RegularExpressions
open System.Xml
open System.Linq
open System.Runtime.Serialization
open Suave
open Suave.Successful
open Suave.Writers
open Suave.Filters
open Suave.Operators
open Suave.Utils
open Suave.DotLiquid
open Utils.SuaveHelpers

open Logic.Commits
open TfsUrlTemplates
open Web.ViewModels

open TestData

module D = Utils.Deserializer

module private Internals =
  // ToDo: Unused function - the old one dates deserializer
  let parseDate str =
    let rx = Regex ("(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d)")
    let m = rx.Match(str)
    let ymd = ["year"; "month"; "day"] |> List.map (fun groupName -> m.Groups.Item(groupName).Value |> int)
    DateTime (ymd.[0], ymd.[1], ymd.[2])

  let renderError (errors: string list) = 
    errors |> String.concat "\n\n" |> OK

  let renderCommits (commits: CommitInfo list) format = 
    let rx = new Regex "\n+"
    let mime = 
      match format with
      | Renderers.Format.Html -> "text/html"
      | Renderers.Format.Txt -> "application/octet-stream"
      |> setMimeType
    commits
    |> List.map (fun ci -> Commit ci)
    |> Renderers.main format
    |> Async.RunSynchronously
    |> fun s -> match format with
                | Renderers.Format.Txt -> rx.Replace (s, "\n")
                | _ -> s
    |> (fun s -> mime >=> OK s)


module Xml = 

  let serializeToXml (o: obj) : string = 
    let sb = new StringBuilder()
    let xmlSerializer = DataContractSerializer(o.GetType())
    xmlSerializer.WriteObject(new XmlTextWriter(new StringWriter(sb)), o)
    sb.ToString()

  let renderXml commits = 
    let header = setHeader "Content-Disposition" "attachment; filename=\"commits.xml\""
    commits
    |> List.map mapCommitInfo
    |> Array.ofList
    |> serializeToXml
    |> (fun s -> header >=> OK s)

module XmlBase = 
  let getXmlWriter (stream: Stream) = 
    //let utf8noBOM = new UTF8Encoding(false)
    let settings = new XmlWriterSettings()
    settings.Indent <- true
    settings.Encoding <- Encoding.UTF8  
    //settings.OmitXmlDeclaration <- true
    XmlWriter.Create(stream, settings)
    
  let streamToString (stream: Stream) = 
    stream.Position <- int64 0
    use sr = new StreamReader(stream)
    sr.ReadToEnd()

  let getResultFromStream (streamWriter: Stream -> unit) = 
    use stream = new MemoryStream ()
    streamWriter stream
    streamToString stream

open Internals

let commitsAction (request: HttpRequest) = 
    let deserializerFn = D.deserializers |> dict |> D.fromString
    [typeof<RepoParams>; typeof<CommitsParams>] 
    |> D.getParamsFromQuery deserializerFn (request.query |> dict)
    |> function
        | Choice2Of2 errors -> errors |> renderError
        | Choice1Of2 objs -> 
          let format = request.query ^^ "format" |> Choice.orDefault "html" |> Renderers.Format.FromString
          let repoParams = objs.[0] :?> RepoParams
          let commitsParams = objs.[1] :?> CommitsParams
          let commits = Logic.Commits.getCommits repoParams commitsParams
          match format with
          | Renderers.Format.Xml -> Xml.renderXml commits
          | _ -> renderCommits commits format
          
let commitsXsd =
  let exporter = XsdDataContractExporter()
  exporter.Export(typeof<CommitInfoXml array>)
  let schemas = exporter.Schemas.Schemas().Cast<Schema.XmlSchema>() |> Array.ofSeq
  let schema = schemas.[1]
  fun s -> s |> XmlBase.getXmlWriter |> schema.Write
  |> XmlBase.getResultFromStream 

let testAction1 () = Renderers.main Renderers.Format.Html (generateActivities())