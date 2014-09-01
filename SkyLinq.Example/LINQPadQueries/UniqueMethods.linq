<Query Kind="Expression" />

typeof(Enumerable).GetMethods().Select(m => m.Name).Distinct().OrderBy(n => n)