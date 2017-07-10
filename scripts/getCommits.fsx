#r @"..\packages\FSharp.Data\lib\net40\FSharp.Data.dll"
#r @"..\packages\DotLiquid\lib\net451\DotLiquid.dll"

#load @"..\WorkLog\DotLiquid.FSharp.fs"

open FSharp.Core
open System
open System.Net
open System.Text
open FSharp.Data
open DotLiquid
open DotLiquid.FSharp



// https://www.visualstudio.com/en-us/docs/integrate/api/git/commits#apageatatime
// [Http authentication in FSharp.Data](https://github.com/fsharp/FSharp.Data/issues/158)

let credentials = NetworkCredential(@"login", "password", "domain")

let addCredentials (req: HttpWebRequest) = 
    req.Credentials <- credentials
    req

let request url = Http.RequestString (url, customizeHttpRequest = addCredentials)

// примеры:
request "https://tfsdsr.norbit.ru/TradingSystems/_apis/git/repositories"
request "https://tfsdsr.norbit.ru/TradingSystems/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2/commits?committer=shmelev"



let apiSettings = 
  dict [
    "host", "tfsdsr.norbit.ru"
    "project", "PurchaseCollection"
  ]

type IApiArgument = 
  abstract member ToString : string

type ArgCommitter = 
  ArgCommitter of string
    interface IApiArgument with
      member x.ToString = 
        let (ArgCommitter str) = x
        str


let commands = 
    [
        "repos", "https://{{host}}/{{project}}/_apis/git/repositories"
        "commits", "https://{{host}}/{{project}}/_apis/git/repositories/{{repo}}/commits?committer=shmelev&fromDate=2017-02-01T00:00:00Z&toDate=2017-02-28T23:59:59Z&$skip=1&$top=2"
    ]



type Task =
  { What : string }
type Person =
  { Name : string
    Tasks : seq<Task> }
let t = parseTemplate<Person> """
  <p>{{ user.Name }} has to do:</p>
  <ul>
    {% for item in user.Tasks -%}
      <li>{{ item.What }}</li>
    {% endfor -%}
  </ul>
"""
t "user" {Name="Tomas"; Tasks = [ {What="sleep"}; {What="eat"} ]}

result

//client.Headers[HttpRequestHeader.Authorization] <- "Basic " + credentials