﻿using ConsoleApp1.StreamTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Paillave.Etl;

namespace ConsoleApp1.Jobs
{
    public class TestJob1 : ExecutionContext<MyConfig>
    {
        public TestJob1() : base("import file")
        {
            var outputFileResourceS = StartupStream.UseResource("open output file", i => new StreamWriter(i.DestinationFilePath));

            var parsedLineS = StartupStream
                .CrossApplyFolderFiles("get folder files", i => i.InputFolderPath, i => i.InputFilesSearchPattern)
                .CrossApplyTextFile("parse input file", new InputFileRowMapper(), (i, p) => { p.FileName = i; return p; })
                .Sort("sort input file", e => e.TypeId);
            //.EnsureSorted("Ensure input file is sorted", i => SortCriteria.Create(i, e => e.TypeId));

            //parsedLineS.ToAction("write to console", i => Console.WriteLine($"{i.FileName} - {i.Id}"));

            var parsedTypeLineS = StartupStream
                .Select("get input file type path", i => i.TypeFilePath)
                .CrossApplyTextFile("parse type input file", new TypeFileRowMapper())
                .EnsureKeyed("Ensure type file is keyed", e => e.Id);

            parsedLineS
                .LeftJoin("join types to file", parsedTypeLineS, (l, r) => new { l.Id, r.Name, l.FileName })
                .Select("output after join", i => new OutputFileRow { FileName = i.FileName, Id = i.Id, Name = i.Name })
                .ToTextFile("write to output file", outputFileResourceS, new OutputFileRowMapper())
                .Select("create text to console", i => $"{i.FileName}:{i.Id}-{i.Name}")
                .ToAction("write to console", Console.WriteLine);
        }
    }
}