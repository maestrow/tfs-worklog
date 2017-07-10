module Domain.Settings

open FSharp.Data

module private Internals = 

  [<Literal>]
  let Snippet = """
  <Settings>
      <ServiceUrl>
          <Schema>string</Schema>
          <Host>string</Host>
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