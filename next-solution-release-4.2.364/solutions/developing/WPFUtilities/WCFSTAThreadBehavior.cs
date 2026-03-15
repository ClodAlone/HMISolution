using System;
using System.Security;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;

namespace Utilities
{
    [AttributeUsage(AttributeTargets.Method)]
    public class STAOperationBehavior : Attribute, System.ServiceModel.Description.IOperationBehavior
    {
        //- @AddBindingParameters -//
        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            //+ blank
        }

        //- @ApplyClientBehavior -//
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            //+ blank
        }

        //- @ApplyDispatchBehavior -//
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new STAInvoker(dispatchOperation.Invoker);
        }

        //- @Validate -//
        public void Validate(OperationDescription operationDescription)
        {
            //+ blank
        }
    }
    public class STAInvoker : System.ServiceModel.Dispatcher.IOperationInvoker
    {
        //- $InnerOperationInvoker -//
        private IOperationInvoker InnerOperationInvoker { get; set; }

        //+
        //- @Ctor -//
        public STAInvoker(IOperationInvoker operationInvoker)
        {
            this.InnerOperationInvoker = operationInvoker;
        }

        //+
        //- @AllocateInputs -//
        public Object[] AllocateInputs()
        {
            return InnerOperationInvoker.AllocateInputs();
        }

        //- @Invoke -//
        public Object Invoke(Object instance, Object[] inputs, out Object[] outputs)
        {
            Exception inner = null;
            Object result = null;
            Object[] staOutputs = null;
            System.ServiceModel.OperationContext context = System.ServiceModel.OperationContext.Current;
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    using (System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(context))
                    {
                        result = InnerOperationInvoker.Invoke(instance, inputs, out staOutputs);
                    }
                }
                catch (Exception ex)
                {
                    inner = ex;
                }
            }));
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (inner != null)
                throw new System.ServiceModel.FaultException(inner.ToString());

            //+
            outputs = staOutputs;
            //+
            return result;
        }

        //- @InvokeBegin -//
        public IAsyncResult InvokeBegin(Object instance, Object[] inputs, AsyncCallback callback, Object state)
        {
            return InnerOperationInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        //- @InvokeEnd -//
        public Object InvokeEnd(Object instance, out Object[] outputs, IAsyncResult result)
        {
            return InnerOperationInvoker.InvokeEnd(instance, out outputs, result);
        }

        //- @IsSynchronous -//
        public bool IsSynchronous
        {
            get { return InnerOperationInvoker.IsSynchronous; }
        }
    }
}
