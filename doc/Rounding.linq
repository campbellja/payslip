<Query Kind="Statements" />

https://stackoverflow.com/questions/14/difference-between-math-floor-and-math-truncate/580252#580252
var raw = 60_050M/12M;
Math.Round(raw,0,MidpointRounding.AwayFromZero).Dump("Math.Round(raw,0,MidpointRounding.AwayFromZero");
Math.Round(raw,0,MidpointRounding.ToEven).Dump("Math.Round(raw,0,MidpointRounding.ToEven");
Math.Ceiling(raw).Dump("Math.Ceiling(raw)");
Math.Floor(raw).Dump("Math.Floor(raw)");