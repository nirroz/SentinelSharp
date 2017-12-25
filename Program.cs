using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Sentinel.Mutators;

namespace Sentinel
{
    class Program
    {
        static int Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.Out.WriteLine("Please specify files or directory.");
                Console.Out.WriteLine("Usage is -files <File Name 1> <File Name 2> <File Name n> or");
                Console.Out.WriteLine("directory name");
                return -1;
            }
			

            FileInfo[] files;
            if (args[0] == "-files")
            {
                var fileList = new List<FileInfo>();
                for (var i = 1; i < args.Length; i++)
                {
                    var file = new FileInfo(args[i]);
                    if (file.Exists)
                    {
                        fileList.Add(file);
                    }
                    else
                    {
                        Console.WriteLine("File not found: " + file.FullName);
                    }
                }

                files = fileList.ToArray();
            }
            else
            {
                var chosenFolder = args[0];
                var chosenDirInfo = new DirectoryInfo(chosenFolder);
                if (!chosenDirInfo.Exists)
                {
                    Console.WriteLine("Error! Folder " + chosenDirInfo.FullName + " doesn't exist!");
                    return -1;
                }

                Console.WriteLine("Running in " + chosenFolder);
                files = chosenDirInfo.GetFiles("*.cs", SearchOption.AllDirectories);
            }

            files.Shuffle();

            var mutators = new List<IMutator>()
            {
                new AlwaysTrueConditionMutator(),
                new RemoveElseMutator(),
                new MethodRemoverMutator(),
            };

            mutators.Shuffle();

            var mutator = mutators[0];
            FileInfo chosenFile = null;
            const string originalFileContentBackup = "OrigianlFileBackUp5935660CCAD54272AA7ED3ACDE7A400C.txt";

            foreach (var currentFile in files)
            {
                if (mutator.ChangedNode != null)
                {
                    break;
                }

                chosenFile = currentFile;

                if (!File.Exists((chosenFile.FullName)))
                {
                    Console.WriteLine("Could not find file " + chosenFile.FullName);
                    continue;
                }

                foreach (var currentMutator in mutators)
                {
                    if (mutator.ChangedNode != null)
                    {
                        break;
                    }

                    mutator = currentMutator;

                    var text = File.ReadAllText(chosenFile.FullName);

                    var res = mutator.MutateRandom(text);
                    switch (res.MutationStatus)
                    {
                        case MutationStatus.Success:
                            Console.WriteLine("Chosen mutator " + mutator.MutationName);
                            Console.WriteLine("Running on " + chosenFile.FullName);

                            File.WriteAllText(chosenFile.FullName, res.NewText);
                            File.WriteAllText(originalFileContentBackup, text);
                            break;
                        case MutationStatus.CantParse:
                        case MutationStatus.NoSuitableMutationFound:
                        default:
                            break;
                    }
                }
            }

            if (mutator.ChangedNode == null)
            {
                Console.WriteLine("No mutations opportunities were found");
                return 1;
            }


            Console.WriteLine("Changes made in line " +
                                  GetLineSpan(mutator).StartLinePosition);
            Console.WriteLine();
            Console.WriteLine();

            if (chosenFile != null)
            {
                var fileNewText = File.ReadAllLines(chosenFile.FullName);
                Console.WriteLine(chosenFile.FullName + " new content:");
                Console.WriteLine("-----------------------------");
                Console.WriteLine();

                var changedStartLine = GetLineSpan(mutator).StartLinePosition.Line;
                var changedEndLine = GetLineSpan(mutator).EndLinePosition.Line;
                for (int i = 0; i < fileNewText.Length; i++)
                {
                    if (i == changedStartLine)
                    {
                        Console.WriteLine(">>>>>>>>>>> " + chosenFile.FullName + " >>>>>>");
                        Console.WriteLine(">>>>>>>>>>> " + mutator.MutationName + " >>>>>>");
                    }

                    Console.WriteLine(fileNewText[i]);

                    if (i == changedEndLine)
                    {
                        Console.WriteLine("<<<<<<<<<<< see original file below <<<<<<<< ");
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("----------O-R-I-G-I-N-A-L----F-I-L-E--------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                
            }


            return 0;
        }

        private static FileLinePositionSpan GetLineSpan(IMutator mutator)
        {
            return mutator.ChangedNode.GetLocation().GetLineSpan();
        }
    }
}
