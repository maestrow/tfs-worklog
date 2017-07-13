module Domain.Settings

open System.IO
open System.Reflection
open FSharp.Data

module private Internals = 

  [<Literal>]
  let Snippet = """
  <Settings>
    <ServiceUrl>
      <Schema>string</Schema>
      <Host>string</Host>
      <Collection>string</Collection>
    </ServiceUrl>
    <NetworkCredential>
      <Login>string</Login>
      <Password>string</Password>
      <Domain>string</Domain>
    </NetworkCredential>
  </Settings>
  """

open Internals

type Settings = XmlProvider<Snippet>

let settings = 
  let root = Path.GetDirectoryName (Assembly.GetExecutingAssembly()).Location
  let xml = Path.Combine (root, "settings.xml") |> File.ReadAllText
  Settings.Parse xml