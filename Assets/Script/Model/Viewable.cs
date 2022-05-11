using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public interface IViewable
    {
        void OnFoundVisibleTarget(Transform target);
        void OnLostAllTargets();
        List<Transform> VisibleTargets();
        bool FoundAnyTarget();
    }
}
