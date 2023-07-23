// Run with:
// dotnet fsi maps.fsx samples/SchoolCodes.txt samples/StudentScoresSchool.txt Or
// dotnet fsi maps.fsx samples/SchoolCodes.txt samples/StudentScoresSchoolExtraCodes.txt Or
// dotnet fsi maps.fsx samples/SchoolCodesAlpha.txt samples/StudentScoresSchoolAlphaCodes.txt

// This script processes a file of student scores and prints a summary of the results.
// The first argument is a file containing a list of school codes.
// The second argument is a file containing a list of student scores.
// Map usage is demonstrated in the SchoolCodes module.

open System
open System.IO

module Float =
 
    let tryFromString s =
        if s = "N/A" then
            None
        else
            Some (float s)
 
    let fromStringOr d s =
        s
        |> tryFromString
        |> Option.defaultValue d
 

module SchoolCodes =

    let load (filePath : string) =
        File.ReadAllLines filePath
        |> Seq.skip 1
        |> Seq.map (fun row -> 
                let elements = row.Split('\t')
                let id = elements.[0] 
                let name = elements.[1]   
                id, name)
        |> Map.ofSeq
        |> Map.add "*" "(External)"      
 
 
type TestResult =
    | Absent
    | Excused
    | Voided
    | Scored of float
 
module TestResult =
 
    let fromString s =
        if s = "A" then
            Absent
        elif s = "E" then
            Excused
        elif s = "V" then
            Voided
        else
            let value = s |> float
            Scored value
 
    let tryEffectiveScore (testResult : TestResult) =
        match testResult with
        | Absent -> Some 0.0
        | Excused 
        | Voided -> None
        | Scored score -> Some score

 
type Student =
    {
        Surname : string
        GivenName : string
        Id : string
        SchoolName : string
        MeanScore : float
        MinScore : float
        MaxScore : float
    }
 
module Student =

    open System.Collections.Generic
 
    let nameParts (s : string) =
        let elements = s.Split(',')
        match elements with
        | [|surname; givenName|] -> 
            {| Surname = surname.Trim()
               GivenName = givenName.Trim() |}
        | [|surname|] ->
            {| Surname = surname.Trim()
               GivenName = "(None)" |}
        | _ -> 
            raise (System.FormatException(sprintf "Invalid student name format: \"%s\"" s))
 
    let fromString (schoolCodes : Map<string, string>) (s : string) =
        let elements = s.Split('\t')
        let name = elements.[0] |> nameParts
        let id = elements.[1]
        let schoolCode = elements.[2]
        let schoolName =
            // match schoolCodes.TryGetValue(schoolCode) with
            // | true, name -> name
            // | false, _ -> "(Unknown school)"
            // OR
            schoolCodes
            |> Map.tryFind(schoolCode)
            |> Option.defaultValue "(Unknown school)"
        let scores =
            elements
            |> Array.skip 3
            |> Array.map TestResult.fromString
            |> Array.choose TestResult.tryEffectiveScore
        let meanScore = scores |> Array.average
        let minScore = scores |> Array.min
        let maxScore = scores |> Array.max
        {
            Surname = name.Surname
            GivenName = name.GivenName
            Id = id
            SchoolName = schoolName
            MeanScore = meanScore
            MinScore = minScore
            MaxScore = maxScore
        }
 
    let printSummary (student : Student) =
        printfn "%s, %s\t%s\t%s\t%0.1f\t%0.1f\t%0.1f" student.Surname student.GivenName student.Id student.SchoolName student.MeanScore student.MinScore student.MaxScore


module Summary =

    let printGroupSummary (surname : string) (students : Student[]) =
        printfn "%s" (surname.ToUpperInvariant())
        students  
        |> Array.sortBy (fun student ->
            student.GivenName, student.Id)
        |> Array.iter (fun student ->
            printfn "\t%20s\t%s\t%0.1f\t%0.1f\t%0.1f"
                student.GivenName student.Id student.MeanScore student.MinScore student.MaxScore)
 
    let summarize schoolCodesFilePath filePath =
        let rows = 
            File.ReadLines filePath
            |> Seq.cache
        let studentCount = (rows |> Seq.length) - 1
        let schoolCodes = SchoolCodes.load schoolCodesFilePath
        printfn "Student count %i" studentCount
        rows
        |> Seq.skip 1
        |> Seq.map (Student.fromString schoolCodes)
        |> Seq.sortByDescending (fun student -> student.MeanScore)
        |> Seq.iter Student.printSummary


let argv = fsi.CommandLineArgs.[1..]

if argv.Length = 2 then
    let schoolCodesFilePath = argv.[0]
    let studentsFilePath = argv.[1]
    if not (File.Exists schoolCodesFilePath) then
        printfn "File not found: %s" schoolCodesFilePath
        1
    elif not (File.Exists studentsFilePath) then
        printfn "File not found: %s" studentsFilePath
        2
    else
        printfn "Processing %s (school codes: %s)"  studentsFilePath schoolCodesFilePath
        try
            Summary.summarize schoolCodesFilePath studentsFilePath
            0
        with
        | :? FormatException as e ->
            printfn "Error: %s" e.Message
            printf "The file was not in the expected format."
            3
        | :? IOException as e ->
            printfn "Error: %s" e.Message
            printfn "If the file is open in another program, please close it."
            4
        | _ as e ->
            printfn "Unexpected error: %s" e.Message
            5
else
    printfn "Please specify a school codes and a students file."
    6
