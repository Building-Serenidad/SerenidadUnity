using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction
{
    public class SelectorDebugVisual : MonoBehaviour
    {
        [SerializeField, Interface(typeof(ISelector))]
        private UnityEngine.Object _selector;

        [SerializeField]
        private Renderer _renderer;

        private ISelector Selector;

        protected bool _started = false;

        protected virtual void Awake()
        {
            Selector = _selector as ISelector;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(Selector, nameof(Selector));
            this.AssertField(_renderer, nameof(_renderer));
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Selector.WhenSelected += HandleSelected;
                Selector.WhenUnselected += HandleUnselected;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                HandleUnselected();
                Selector.WhenSelected -= HandleSelected;
                Selector.WhenUnselected -= HandleUnselected;
            }
        }

        private void OnDestroy()
        {
            // Clean up
        }

        private void HandleSelected()
        {
            _renderer.enabled = true; // Make the object visible
        }

        private void HandleUnselected()
        {
            _renderer.enabled = false; // Make the object invisible
        }

        #region Inject

        public void InjectAllSelectorDebugVisual(ISelector selector, Renderer renderer)
        {
            InjectSelector(selector);
            InjectRenderer(renderer);
        }

        public void InjectSelector(ISelector selector)
        {
            _selector = selector as UnityEngine.Object;
            Selector = selector;
        }

        public void InjectRenderer(Renderer renderer)
        {
            _renderer = renderer;
        }

        #endregion
    }
}
