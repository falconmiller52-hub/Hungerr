using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Common.Services.Updateable
{
    public class UpdateableService : MonoBehaviour, IUpdateableService
    {
        private List<IUpdateable>  _updateables;

        public void AddUpdateable(IUpdateable updateable)
        {
            if (_updateables != null && !_updateables.Contains(updateable))
                _updateables.Add(updateable);
        }

        public void RemoveUpdateable(IUpdateable updateable)
        {
            if (_updateables != null && _updateables.Contains(updateable))
                _updateables.Remove(updateable);
        }

        private void Start()
        {
            _updateables = new List<IUpdateable>();
        }

        void Update()
        {
            foreach (var updateable in _updateables)
                updateable.Update();
        }

        public void Dispose()
        {
            _updateables = null;
        }
    }
}
