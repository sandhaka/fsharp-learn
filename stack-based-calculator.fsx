// Project tutorial:
// https://fsharpforfunandprofit.com/posts/stack-based-calculator/

type Stack = StackContents of float list
let newStack = StackContents [1.0; 2.0; 3.0] // Is a list

let push x (StackContents contents) =
    StackContents (x :: contents)

let pop (StackContents contents) =
    match contents with
    | [] -> failwith "Stack is empty"
    | top::rest -> top, StackContents rest

let binary (mathFn : float -> float -> float) (stack : Stack) =
    let x, stack' = pop stack
    let y, stack'' = pop stack'
    push (mathFn x y) stack''
    
let Add (stack : Stack) = binary (+) stack
let Mul (stack : Stack) = binary (*) stack
let Sub (stack : Stack) = binary (-) stack
let Div (stack : Stack) = binary (/) stack

let unary f stack =
    let x, stack' = pop stack
    push (f x) stack'

let Neg (stack : Stack) = unary (fun x -> -x) stack
let Square (stack : Stack) = unary (fun x -> x * x) stack

let show (stack : Stack) =
    let x, _ = pop stack
    printfn $"Result: %f{x}"
    stack

// Sample usage

let EMPTY = StackContents []
let ONE = push 1.0
let TWO = push 2.0
let THREE = push 3.0
let FOUR = push 4.0
let FIVE = push 5.0

let add_test = EMPTY |> ONE |> FIVE |> Add
add_test |> show

let mul_test = EMPTY |> ONE |> FIVE |> Mul
mul_test |> show

let sub_test = EMPTY |> FOUR |> TWO |> Sub
sub_test |> show

let div_test = EMPTY |> FOUR |> TWO |> Div
div_test |> show

let neg_test = EMPTY |> ONE |> Neg
neg_test |> show

let square_test = EMPTY |> TWO |> Square
square_test |> show

let complex_test = EMPTY |> ONE |> TWO |> Add |> Neg |> Square
complex_test |> show
