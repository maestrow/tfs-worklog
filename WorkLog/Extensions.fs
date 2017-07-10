module Extensions

open System

type DateTime with
  member this.GetMonthBoundaries () = 
    let dateFrom = DateTime(this.Year, this.Month, 1)
    let dateTo = DateTime(this.Year, this.Month, DateTime.DaysInMonth(this.Year, this.Month))
    (dateFrom, dateTo)