// This script reads a tab-delimited file and creates a read-only dictionary
// from the contents. The first line of the file is skipped.

open System.IO

let args = fsi.CommandLineArgs.[1..]

File.ReadAllLines args.[0]
    |> Seq.skip 1
    |> Seq.map (fun row ->
        let elements = row.Split('\t')
        let id = elements.[0]
        let name = elements.[1]
        id, name)
    |> readOnlyDict
    |> printfn "%A"

