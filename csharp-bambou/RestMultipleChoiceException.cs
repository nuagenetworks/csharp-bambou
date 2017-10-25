/*
  Copyright (c) 2017, Nokia
  All rights reserved.

  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions are met:
      * Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.
      * Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.
      * Neither the name of the copyright holder nor the names of its contributors
        may be used to endorse or promote products derived from this software without
        specific prior written permission.

  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY
  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.nuagenetworks.bambou
{
    public class ResponseChoice
    {
        private string label;
        private int id;

        public string Label { get => label; set => label = value; }
        public int Id { get => id; set => id = value; }
    }

    public class RestMultipleChoiceException: RestException
    {

        private string title;
        private string description;
        private List<ResponseChoice> choices;

        public RestMultipleChoiceException(string data) : base("Multiple Choices")
        {
            dynamic root = JObject.Parse(data);
            this.title = root.errors[0].descriptions[0].title;
            this.description = root.errors[0].descriptions[0].description;

            this.choices = new List<ResponseChoice>();
            foreach (dynamic choice in root.choices)
            {
                ResponseChoice rc = new ResponseChoice();
                rc.Label = choice.label;
                rc.Id = choice.id;
                this.choices.Add(rc);
            }

        }

        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public List<ResponseChoice> Choices { get => choices; set => choices = value; }
    }
}
