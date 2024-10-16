﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GladcherryShopping.Sep.Refrence.Payment {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:Foo", ConfigurationName="Sep.Refrence.Payment.PaymentIFBindingSoap")]
    public interface PaymentIFBindingSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="verifyTransaction", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        double verifyTransaction(string String_1, string String_2);
        
        [System.ServiceModel.OperationContractAttribute(Action="verifyTransaction", ReplyAction="*")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        System.Threading.Tasks.Task<double> verifyTransactionAsync(string String_1, string String_2);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        double verifyTransaction1(string String_1, string String_2);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        System.Threading.Tasks.Task<double> verifyTransaction1Async(string String_1, string String_2);
        
        [System.ServiceModel.OperationContractAttribute(Action="reverseTransaction", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        double reverseTransaction(string String_1, string String_2, string Username, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="reverseTransaction", ReplyAction="*")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        System.Threading.Tasks.Task<double> reverseTransactionAsync(string String_1, string String_2, string Username, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="reverseTransaction1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        double reverseTransaction1(string String_1, string String_2, string Password, double Amount);
        
        [System.ServiceModel.OperationContractAttribute(Action="reverseTransaction1", ReplyAction="*")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        System.Threading.Tasks.Task<double> reverseTransaction1Async(string String_1, string String_2, string Password, double Amount);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface PaymentIFBindingSoapChannel : GladcherryShopping.Sep.Refrence.Payment.PaymentIFBindingSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PaymentIFBindingSoapClient : System.ServiceModel.ClientBase<GladcherryShopping.Sep.Refrence.Payment.PaymentIFBindingSoap>, GladcherryShopping.Sep.Refrence.Payment.PaymentIFBindingSoap {
        
        public PaymentIFBindingSoapClient() {
        }
        
        public PaymentIFBindingSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PaymentIFBindingSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PaymentIFBindingSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PaymentIFBindingSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public double verifyTransaction(string String_1, string String_2) {
            return base.Channel.verifyTransaction(String_1, String_2);
        }
        
        public System.Threading.Tasks.Task<double> verifyTransactionAsync(string String_1, string String_2) {
            return base.Channel.verifyTransactionAsync(String_1, String_2);
        }
        
        public double verifyTransaction1(string String_1, string String_2) {
            return base.Channel.verifyTransaction1(String_1, String_2);
        }
        
        public System.Threading.Tasks.Task<double> verifyTransaction1Async(string String_1, string String_2) {
            return base.Channel.verifyTransaction1Async(String_1, String_2);
        }
        
        public double reverseTransaction(string String_1, string String_2, string Username, string Password) {
            return base.Channel.reverseTransaction(String_1, String_2, Username, Password);
        }
        
        public System.Threading.Tasks.Task<double> reverseTransactionAsync(string String_1, string String_2, string Username, string Password) {
            return base.Channel.reverseTransactionAsync(String_1, String_2, Username, Password);
        }
        
        public double reverseTransaction1(string String_1, string String_2, string Password, double Amount) {
            return base.Channel.reverseTransaction1(String_1, String_2, Password, Amount);
        }
        
        public System.Threading.Tasks.Task<double> reverseTransaction1Async(string String_1, string String_2, string Password, double Amount) {
            return base.Channel.reverseTransaction1Async(String_1, String_2, Password, Amount);
        }
    }
}
