using UnityEngine;
using System.Collections;

namespace UniExtensions.Resource {
    public class Recycler : MonoBehaviour {
        public PrefabPool pool;
        public void Recycle() {
            pool.Recycle(gameObject);
        }
    }
}
