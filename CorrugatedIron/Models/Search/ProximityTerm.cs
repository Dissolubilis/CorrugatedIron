﻿// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System.Collections.Generic;
using System.Linq;

namespace CorrugatedIron.Models.Search
{
    public class ProximityTerm : Term
    {
        private readonly List<Token> _words;
        private readonly double _proximity;

        public ProximityTerm(RiakFluentSearch search, string field, double proximity, params string[] words)
            : base(search, field)
        {
            _words = new List<Token>(words.Select(Token.Is));
            _proximity = proximity;
        }

        public override string ToString()
        {
            return Prefix() + Field() + "\"" + string.Join(" ", _words.Select(w => w.ToString()).ToArray()) + "\"~" + _proximity + Suffix();
        }
    }
}