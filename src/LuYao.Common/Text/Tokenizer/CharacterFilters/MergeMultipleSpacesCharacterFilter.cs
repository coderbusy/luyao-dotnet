using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LuYao.Text.Tokenizer.CharacterFilters;

public class MergeMultipleSpacesCharacterFilter : RegexCharacterFilter
{
    public MergeMultipleSpacesCharacterFilter() : base(new Regex(@"\s+", RegexOptions.Compiled), " ")
    {

    }
}
