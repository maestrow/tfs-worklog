(*
Функционал генерации тестовых данных из библиотеки FsCheck с первого взгляда для меня показался слишком запутанным.
Изучение FsCheck я отложил и написал для нужд негерации данных более простой в использовании и в освоении модуль.
*)

module TestDataGenerators

open System

type Gen<'t> = unit -> 't

module Gen = 
  let intRange min max : Gen<int> =
    let r = Random ()
    fun () -> 
      r.Next (min, max)

  let guid : Gen<Guid> = 
    fun () -> 
      Guid.NewGuid ()