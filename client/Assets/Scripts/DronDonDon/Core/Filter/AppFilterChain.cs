using System.Collections.Generic;
using IoC;
using UnityEngine;

namespace DronDonDon.Core.Filter
{
    public class AppFilterChain : MonoBehaviour
    {
        private readonly Queue<IAppFilter> _filters = new Queue<IAppFilter>();

        public void AddFilter(IAppFilter filter)
        {
            _filters.Enqueue(filter);
        }

        public void Next()
        {
            if (_filters.Count == 0) {
                Destroy(this);
                return;
            }
            IAppFilter filter = _filters.Dequeue();
            if (AppContext.Container != null) {
                AppContext.Inject(filter);
            }

            filter.Run(this);
        }
    }
}