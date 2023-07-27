// Discriminated unions
// https://fsharpforfunandprofit.com/posts/discriminated-unions/

// Let define a type of int or bool
type IntOrBool = I of int | B of bool

let i  = I 99    // use the "I" constructor
// val i : IntOrBool = I 99

let b  = B true  // use the "B" constructor
// val b : IntOrBool = B true

// The component types can be any other type you like, including tuples, records, other union types, and so on.
type Person = { first : string; last : string }
type MixedType =
    | Tup of int * int
    | P of Person
    | L of int list
    | U of IntOrBool
