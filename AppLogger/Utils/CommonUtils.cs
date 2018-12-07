using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogger.Utils
{
    static class CommonUtils
    {
        //Разбивает список на несколько списков по BatchSize штук
        public static List<List<T>> SplitIntoBatches<T>(this List<T> list, int BatchSize)
        {
            var count = list.Count();
            var BatchCount = count / BatchSize;

            var slices = new List<List<T>>();

            for (int i = 0; i <= BatchCount; i++)
            {   
                int sliceSize;
                if (BatchCount == i)
                {
                    sliceSize = list.Count - BatchSize * i;
                }
                else
                {
                    sliceSize = BatchSize;
                }
                slices.Add(list.GetRange(BatchSize * i, sliceSize));
            }

            return slices;
        }

    }
}
