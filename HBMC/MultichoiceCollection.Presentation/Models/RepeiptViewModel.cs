using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultichoiceCollection.Common.Entities;
using MultichoiceCollection.Common.Entities.Base;

namespace MultichoiceCollection.Presentation.Models
{
    public class RepeiptViewModel
    {
        public ApplicationUser User { get; set; }
        public Transaction Transaction { get; set; }
    }
}