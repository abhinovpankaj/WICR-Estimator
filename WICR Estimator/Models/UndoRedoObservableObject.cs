using MyToolkit.Collections;
using MyToolkit.Model;
using MyToolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    [DataContract]  
    public class UndoRedoObservableObject : NotifiableObject
    {
        private readonly List<object> _registeredChildren = new List<object>();
        private readonly Dictionary<object, List<object>> _registeredCollections = new Dictionary<object, List<object>>();
        private readonly List<Type> _excludedChildTypes = new List<Type>();
        
        private bool _suppressGraphPropertyChanged = false;

        /// <summary>Occurs when a property value of the object or any child changes. </summary>
        public event PropertyChangedEventHandler GraphPropertyChanged;

        /// <summary>Gets the child types which are excluded for graph tracking (direct references or in collections).</summary>
        protected List<Type> ExcludedChildTypes
        {
            get { return _excludedChildTypes; }
        }

        /// <summary>Updates the property and raises the changed event, but only if the new value does not equal the old value. </summary>
        /// <param name="propertyName">The property name as lambda. </param>
        /// <param name="oldValue">A reference to the backing field of the property. </param>
        /// <param name="newValue">The new value. </param>
        /// <returns>True if the property has changed. </returns>
        /// 

        public override bool Set<T>(string propertyName, ref T oldValue, T newValue)
        {
            if (Equals(oldValue, newValue))
                return false;

            DeregisterChild(oldValue);
            RegisterChild(newValue);

            var args = new GraphPropertyChangedEventArgs(propertyName, oldValue, newValue);

            oldValue = newValue;
            RaisePropertyChanged(args);
            return true;
        }

        /// <summary>Raises the property changed event with <see cref="GraphPropertyChangedEventArgs"/> arguments. </summary>
        /// <param name="oldValue">The old value. </param>
        /// <param name="newValue">The new value. </param>
        /// <param name="propertyName">The property name. </param>
        public void RaisePropertyChanged(object oldValue, object newValue, [CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged(new GraphPropertyChangedEventArgs(propertyName, oldValue, newValue));
        }

        /// <summary>Raises the property changed event with <see cref="GraphPropertyChangedEventArgs"/> arguments. </summary>
        /// <param name="oldValue">The old value. </param>
        /// <param name="newValue">The new value. </param>
        /// <param name="propertyNameExpression">The property name as lambda. </param>
        public void RaisePropertyChanged(Expression<Func<object>> propertyNameExpression, object oldValue, object newValue)
        {
            RaisePropertyChanged(new GraphPropertyChangedEventArgs(ExpressionUtilities.GetPropertyName(propertyNameExpression), oldValue, newValue));
        }

        /// <summary>Raises the property changed event with <see cref="GraphPropertyChangedEventArgs"/> arguments. </summary>
        /// <param name="oldValue">The old value. </param>
        /// <param name="newValue">The new value. </param>
        /// <typeparam name="TClass">The type of the class with the property. </typeparam>
        /// <param name="propertyNameExpression">The property name as lambda. </param>
        public void RaisePropertyChanged<TClass>(Expression<Func<TClass, object>> propertyNameExpression, object oldValue, object newValue)
        {
            RaisePropertyChanged(new GraphPropertyChangedEventArgs(ExpressionUtilities.GetPropertyName(propertyNameExpression), oldValue, newValue));
        }

        /// <summary>Registers a child to receive property changes. </summary>
        /// <param name="child">The child object. </param>
        protected void RegisterChild(object child)
        {

            if (child == null)
                return;

            try
            {
#if LEGACY
            var childTypeInfo = child.GetType();
            if (_registeredChildren.Contains(child) || ExcludedChildTypes.Any(t => t.IsAssignableFrom(childTypeInfo)))
                return;
#else
                var childTypeInfo = child.GetType().GetTypeInfo();
                if (_registeredChildren.Contains(child) || ExcludedChildTypes.Any(t => t.GetTypeInfo().IsAssignableFrom(childTypeInfo)))
                    return;
#endif
                if (child is UndoRedoObservableObject)
                {
                    ((UndoRedoObservableObject)child).GraphPropertyChanged += RaiseGraphPropertyChanged;
                    _registeredChildren.Add(child);
                    if (child is SlopeBaseViewModel)
                    {
                        var newChild = ((SlopeBaseViewModel)child).Slopes;
                    if (newChild is ICollection)
                        {
                            var list = newChild as ICollection;
                            foreach (var item in list)
                                RegisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                                _registeredCollections.Add(child, list.OfType<object>().ToList());
                            }
                        }
                    }
                    if (child is PedestrianSlopeViewModel)
                    {
                        var newChild = ((PedestrianSlopeViewModel)child).UrethaneSlopes;
                        if (newChild is ICollection)
                        {
                            var list = newChild as ICollection;
                            foreach (var item in list)
                                RegisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                    }
                    if (child is DualFlexSlopeViewModel)
                    {
                        var newChild = ((DualFlexSlopeViewModel)child).UrethaneSlopes;
                        if (newChild is ICollection)
                        {
                            var list = newChild as ICollection;
                            foreach (var item in list)
                                RegisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                    }

                    //Material
                    if (child is MaterialBaseViewModel)
                    {
                        var newChild = ((MaterialBaseViewModel)child).SystemMaterials;
                        if (newChild is ICollection)
                        {
                            var list = newChild as ICollection;
                            foreach (var item in list)
                                RegisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                        var newChild1 = ((MaterialBaseViewModel)child).OtherLaborMaterials;
                        if (newChild1 is ICollection)
                        {
                            var list = newChild1 as ICollection;
                            foreach (var item in list)
                                RegisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                        var newChild2 = ((MaterialBaseViewModel)child).OtherMaterials;
                        if (newChild2 is ICollection)
                        {
                            var list = newChild2 as ICollection;
                            foreach (var item in list)
                                RegisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                    }
                    //Metal
                    if (child is MetalBaseViewModel)
                    {
                        var metalChild = ((MetalBaseViewModel)child).Metals;
                        if (metalChild is ICollection)
                        {
                            var list = metalChild as ICollection;
                            foreach (var item in list)
                                RegisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                        var metalChild1 = ((MetalBaseViewModel)child).AddOnMetals;
                        if (metalChild1 is ICollection)
                        {
                            var list = metalChild1 as ICollection;
                            foreach (var item in list)
                                RegisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                    }

                }
                else if (child is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)child).PropertyChanged += RaiseGraphPropertyChanged;
                    _registeredChildren.Add(child);

                    if (child is ICollection)
                    {
                        var list = child as ICollection;
                        foreach (var item in list)
                            RegisterChild(item);

                        if (child is INotifyCollectionChanged)
                        {
                            ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                            _registeredCollections.Add(child, list.OfType<object>().ToList());
                        }
                    }

                }
                else if (child.GetType().Name == "KeyValuePair`2")
                {
                    // TODO: [PERF] add cache
#if LEGACY
                var value = child.GetType().GetProperty("Value").GetValue(child, null); 
#else
                    var value = child.GetType().GetRuntimeProperty("Value").GetValue(child);
#endif
                    RegisterChild(value);
                }
            }
            catch (Exception)
            {

                
            }

            
        }

        /// <summary>Deregisters a child. </summary>
        /// <param name="child">The child object. </param>
        protected void DeregisterChild(object child)
        {
            if (_registeredChildren == null)
                return;
            if (ExcludedChildTypes == null)
                return;

            try
            {
                if (child == null || !_registeredChildren.Contains(child))
                    return;

                if (child is UndoRedoObservableObject)
                {
                    ((UndoRedoObservableObject)child).GraphPropertyChanged -= RaiseGraphPropertyChanged;
                    _registeredChildren.Remove(child);
                    //Slopebase
                    if (child is SlopeBaseViewModel)
                    {
                        var newChild = ((SlopeBaseViewModel)child).Slopes;
                        if (newChild is ICollection)
                        {
                            var list = newChild as ICollection;
                            foreach (var item in list)
                                DeregisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                    }
                    if (child is PedestrianSlopeViewModel)
                    {
                        var newChild = ((PedestrianSlopeViewModel)child).UrethaneSlopes;
                        if (newChild is ICollection)
                        {
                            var list = newChild as ICollection;
                            foreach (var item in list)
                                DeregisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                    }
                    if (child is DualFlexSlopeViewModel)
                    {
                        var newChild = ((DualFlexSlopeViewModel)child).UrethaneSlopes;
                        if (newChild is ICollection)
                        {
                            var list = newChild as ICollection;
                            foreach (var item in list)
                                DeregisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                    }

                    //Material
                    if (child is MaterialBaseViewModel)
                    {
                        var newChild = ((MaterialBaseViewModel)child).SystemMaterials;
                        if (newChild is ICollection)
                        {
                            var list = newChild as ICollection;
                            foreach (var item in list)
                                DeregisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                        var newChild1 = ((MaterialBaseViewModel)child).OtherLaborMaterials;
                        if (newChild1 is ICollection)
                        {
                            var list = newChild1 as ICollection;
                            foreach (var item in list)
                                DeregisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                        var newChild2 = ((MaterialBaseViewModel)child).OtherMaterials;
                        if (newChild2 is ICollection)
                        {
                            var list = newChild2 as ICollection;
                            foreach (var item in list)
                                DeregisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                    }
                    //Metal
                    if (child is MetalBaseViewModel)
                    {
                        var metalChild = ((MetalBaseViewModel)child).Metals;
                        if (metalChild is ICollection)
                        {
                            var list = metalChild as ICollection;
                            foreach (var item in list)
                                DeregisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                        var metalChild1 = ((MetalBaseViewModel)child).AddOnMetals;
                        if (metalChild1 is ICollection)
                        {
                            var list = metalChild1 as ICollection;
                            foreach (var item in list)
                                DeregisterChild(item);

                            if (child is INotifyCollectionChanged)
                            {
                                ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                                _registeredCollections.Remove(child);
                            }
                        }
                    }

                }
                else if (child is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)child).PropertyChanged -= RaiseGraphPropertyChanged;
                    _registeredChildren.Remove(child);

                    if (child is ICollection)
                    {
                        var list = child as ICollection;
                        foreach (var item in list)
                            DeregisterChild(item);

                        if (child is INotifyCollectionChanged)
                        {
                            ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                            _registeredCollections.Remove(child);
                        }
                    }
                }
                else if (child.GetType().Name == "KeyValuePair`2")
                {
                    // TODO: [PERF] add cache
#if LEGACY
                var value = child.GetType().GetProperty("Value").GetValue(child, null); 
#else
                    var value = child.GetType().GetRuntimeProperty("Value").GetValue(child);
#endif
                    DeregisterChild(value);
                }
            }
            catch (Exception ex )
            {

                throw;
            }
            
        }

        /// <summary>Raises the property changed event. </summary>
        /// <param name="args">The arguments. </param>
        protected override void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            base.RaisePropertyChanged(args);
            RaiseGraphPropertyChanged(this, args);
        }

        private void RaiseGraphPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_suppressGraphPropertyChanged)
                return;

            _suppressGraphPropertyChanged = true; // used to avoid multiple calls in cyclic graphs

            var copy = GraphPropertyChanged;
            if (copy != null)
                copy(sender, args);

            _suppressGraphPropertyChanged = false;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var oldCollection = _registeredCollections[sender];
            var oldCollectionCopy = oldCollection.ToList();

            var addedItems = new List<object>();
            foreach (var item in ((ICollection)sender).OfType<object>().Where(i => !oldCollection.Contains(i))) // new items
            {
                addedItems.Add(item);
                oldCollection.Add(item);
                RegisterChild(item);
            }

            var removedItems = new List<object>();
            var currentItems = ((ICollection)sender).OfType<object>().ToArray();
            foreach (var item in oldCollection.Where(i => !currentItems.Contains(i)).ToArray()) // deleted items
            {
                removedItems.Add(item);
                oldCollection.Remove(item);
                DeregisterChild(item);
            }

            RaiseGraphPropertyChanged(sender, new MtNotifyCollectionChangedEventArgs<object>(addedItems, removedItems, oldCollectionCopy));
        }
    }
}
