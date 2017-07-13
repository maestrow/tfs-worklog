module Logic.TfsRequest

open System
open System.Net
open FSharp.Data
open Domain.Settings

module private Implementation = 

  let credentials = 
      let c = settings.NetworkCredential
      NetworkCredential(c.Login, c.Password, c.Domain)

  let addCredentials (req: HttpWebRequest) = 
      req.Credentials <- credentials
      req

open Implementation

let request url = Http.RequestString (url, customizeHttpRequest = addCredentials)

