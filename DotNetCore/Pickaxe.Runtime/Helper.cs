/* Copyright 2015 Brock Reeve
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime
{
    public static class Helper
    {
        public static string NullConcat(object arg0, object arg1)
        {
            if (arg0 == null || arg1 == null)
                return null;

            return string.Concat(arg0, arg1);
        }

        public static bool Like(string expression, string likeString)
        {
            const string Percent = "%";
            if (likeString.StartsWith(Percent) && likeString.EndsWith(Percent))
            {
                likeString = likeString.Replace(Percent, "");
                return expression.Contains(likeString);
            }
            else if (likeString.StartsWith(Percent))
            {
                likeString = likeString.Replace(Percent, "");
                return expression.StartsWith(likeString);
            }
            else if (likeString.EndsWith(Percent))
            {
                likeString = likeString.Replace(Percent, "");
                return expression.EndsWith(likeString);
            }

            likeString = likeString.Replace(Percent, "");
            return expression == likeString;
        }
    }
}
