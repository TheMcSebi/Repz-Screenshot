using RepzScreenshot.Model;
using RepzScreenshot.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RepzScreenshot.DataAccess
{
    class DataAccessBase
    {


        public async Task UpdateCollection<T, U>(ObservableCollection<T> collection, Func<Task<List<U>>> newData, Func<T, U> instance, Func<U, IComparable> cond, Action<U> add)
            where T : ViewModelBase
            where U : ModelBase
        {

            List<U> newItems = await newData();
            List<U> toRemove = new List<U>(collection.Select(x => instance(x)));
            List<U> toAdd = new List<U>(newItems.Where(x => !collection.Any(y => cond(x).Equals(cond(instance(y))))));

            
            //remove not longer existing items
            toRemove.RemoveAll(x => newItems.Any(y => cond(y).Equals(cond(x))));
            foreach (var tr in toRemove)
            {
                collection.Remove(collection.First(x => cond(instance(x)).Equals(cond(tr))));
            }

            //update still existing items
            foreach (T vm in collection)
            {
                U i = newItems.First(x => cond(x).Equals(cond(instance(vm))));
                instance(vm).Update(i);
            }

            //add new items
            toAdd.ForEach(x => add(x));
        }
    }
}
