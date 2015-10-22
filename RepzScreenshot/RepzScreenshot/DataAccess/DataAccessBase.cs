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


        public async Task UpdateCollection<T, U>(ObservableCollection<T> collection, Func<Task<List<U>>> newData, Func<T, U> instance, Action<U> add, bool remove)
            where T : ViewModelBase
            where U : ModelBase, IEquatable<U>
        {
            try
            {
                List<U> newItems = await newData();
                List<U> currentItems = collection.Select(x => instance(x)).ToList();
                List<U> toRemove = new List<U>();
                List<U> toAdd = newItems.Where(x => !currentItems.Any(y => x.Equals(y))).ToList();
                List<T> toUpdate = collection.Where(x => newItems.Any(y => y.Equals(instance(x)))).ToList();


                if (remove)
                {
                    toRemove = currentItems.Where(x => !newItems.Any(y => x.Equals(y))).ToList();
                }


                //remove items
                foreach (U tr in toRemove)
                {
                    collection.Remove(collection.First(x => instance(x).Equals(tr)));
                }


                //update still existing items
                foreach (T vm in toUpdate)
                {
                    U item = instance(vm);
                    item.Update(newItems.First(x => x.Equals(item)));

                    collection.Remove(vm);
                    collection.Add(vm);
                }


                //add new items
                toAdd.ForEach(x => add(x));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
