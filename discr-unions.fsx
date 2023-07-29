// Discriminated unions
// https://fsharpforfunandprofit.com/posts/discriminated-unions/

// Let define a type of int or bool
type IntOrBool = I of int | B of bool

let i  = I 99    // use the "I" constructor
// val i : IntOrBool = I 99

let b  = B true  // use the "B" constructor
// val b : IntOrBool = B true

// The component types can be any other type you like, including tuples,
// records, other union types, and so on.
type Person = { first : string; last : string }
type MixedType =
    | Tup of int * int
    | P of Person
    | L of int list
    | U of IntOrBool
let personValue = P { first = "John"; last = "Smith" }
let tupleValue = Tup (1, 2)
let listValue = L [1; 2; 3]
let unionValue = U (I 99)
let unionValue2 = U (B true)
// Is much better to explicitly qualify the type:
personValue = MixedType.P { first = "John"; last = "Smith" }

// Matching and deconstructing
let matcher x =
    match x with
    | Tup (a, b) ->
        printfn $"Tuple: (%d{a}, %d{b})"
    | P { first = f; last = l } ->
        printfn $"Person: %s{f} %s{l}"
    | L list ->
        printfn $"List of int: %A{list}"
    | U intOrBool ->
        printfn "IntOrBool: "
        match intOrBool with
        | I i ->
            printfn $"  Integer: %d{i}"
        | B b ->
            printfn $"  Boolean: %b{b}"

// Use the matcher function
matcher personValue
matcher tupleValue
matcher listValue
matcher unionValue
matcher unionValue2

// Empty cases
type Result =
    | Success
    | Failure of string
    
let result = Success
let result2 = Failure "Something went wrong"
