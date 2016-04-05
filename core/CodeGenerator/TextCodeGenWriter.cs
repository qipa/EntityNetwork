﻿using System;
using System.Linq;
using System.Text;

namespace CodeGen
{
    public class TextCodeGenWriter : ICodeGenWriter
    {
        private StringBuilder _header = new StringBuilder();
        private StringBuilder _body = new StringBuilder();
        private int _indent;
        private bool _newLineNeeded;

        private string IndentStr
        {
            get { return string.Concat(Enumerable.Repeat("    ", _indent)); }
        }

        private void SetNewLineNeeded(bool value)
        {
            _newLineNeeded = value;
        }

        private void EnsureNewLine()
        {
            if (_newLineNeeded)
            {
                _newLineNeeded = false;
                _body.Append("\n");
            }
        }

        public void AddUsing(string @namespace)
        {
            if (_indent == 0)
            {
                _header.AppendFormat("using {0};\n", @namespace);
            }
            else
            {
                _body.AppendFormat(IndentStr + "using {0};\n", @namespace);
            }
        }

        public void AddCode(string code)
        {
            EnsureNewLine();

            foreach (var line in code.Split('\n'))
            {
                var pureLine = line.TrimEnd();
                if (pureLine.Length == 0)
                    _body.AppendLine("");
                else
                    _body.AppendLine(IndentStr + pureLine.Replace("\t", "    "));
            }

            SetNewLineNeeded(true);
        }

        public void PushNamespace(string @namespace)
        {
            EnsureNewLine();

            _body.AppendFormat(IndentStr + "namespace {0}\n", @namespace);
            _body.Append(IndentStr + "{\n");
            _indent += 1;

            SetNewLineNeeded(false);
        }

        public void PopNamespace()
        {
            _indent -= 1;
            _body.Append(IndentStr + "}\n");

            SetNewLineNeeded(true);
        }

        public void PushRegion(string @region)
        {
            EnsureNewLine();

            _body.AppendFormat(IndentStr + "#region {0}\n", @region);

            SetNewLineNeeded(true);
        }

        public void PopRegion()
        {
            EnsureNewLine();

            _body.Append(IndentStr + "#endregion\n");

            SetNewLineNeeded(true);
        }

        public override string ToString()
        {
            var comment =
                @"
// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Akka.Interfaced CodeGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
                ".Trim();
            var src = comment + "\n\n" + _header.ToString() + "\n" + _body.ToString();
            return src.Replace("\r", "");
        }
    }
}
