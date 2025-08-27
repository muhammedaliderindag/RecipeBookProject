using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class VoteRecipeDto
    {
        public int productid { get; set; } = 0;
        public int vote { get; set; } = 0;

        public double avgVote { get; set; } = 1;
        public int totalVoters { get; set; } = 0;
    }
}
