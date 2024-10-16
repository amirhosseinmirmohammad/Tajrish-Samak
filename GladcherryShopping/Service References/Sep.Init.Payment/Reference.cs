﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GladcherryShopping.Sep.Init.Payment {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:Foo", ConfigurationName="Sep.Init.Payment.PaymentIFBindingSoap")]
    public interface PaymentIFBindingSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="RequestToken", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        string RequestToken(string TermID, string ResNum, long TotalAmount, long SegAmount1, long SegAmount2, long SegAmount3, long SegAmount4, long SegAmount5, long SegAmount6, string AdditionalData1, string AdditionalData2, long Wage);
        
        [System.ServiceModel.OperationContractAttribute(Action="RequestToken", ReplyAction="*")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        System.Threading.Tasks.Task<string> RequestTokenAsync(string TermID, string ResNum, long TotalAmount, long SegAmount1, long SegAmount2, long SegAmount3, long SegAmount4, long SegAmount5, long SegAmount6, string AdditionalData1, string AdditionalData2, long Wage);
        
        [System.ServiceModel.OperationContractAttribute(Action="RequestMultiSettleTypeToken", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        string RequestMultiSettleTypeToken(string TermID, string ResNum, string Amounts, long TotalAmount, long SegAmount1, long SegAmount2, long SegAmount3, long SegAmount4, long SegAmount5, long SegAmount6, string AdditionalData1, string AdditionalData2, long Wage);
        
        [System.ServiceModel.OperationContractAttribute(Action="RequestMultiSettleTypeToken", ReplyAction="*")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        System.Threading.Tasks.Task<string> RequestMultiSettleTypeTokenAsync(string TermID, string ResNum, string Amounts, long TotalAmount, long SegAmount1, long SegAmount2, long SegAmount3, long SegAmount4, long SegAmount5, long SegAmount6, string AdditionalData1, string AdditionalData2, long Wage);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface PaymentIFBindingSoapChannel : GladcherryShopping.Sep.Init.Payment.PaymentIFBindingSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PaymentIFBindingSoapClient : System.ServiceModel.ClientBase<GladcherryShopping.Sep.Init.Payment.PaymentIFBindingSoap>, GladcherryShopping.Sep.Init.Payment.PaymentIFBindingSoap {
        
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
        
        public string RequestToken(string TermID, string ResNum, long TotalAmount, long SegAmount1, long SegAmount2, long SegAmount3, long SegAmount4, long SegAmount5, long SegAmount6, string AdditionalData1, string AdditionalData2, long Wage) {
            return base.Channel.RequestToken(TermID, ResNum, TotalAmount, SegAmount1, SegAmount2, SegAmount3, SegAmount4, SegAmount5, SegAmount6, AdditionalData1, AdditionalData2, Wage);
        }
        
        public System.Threading.Tasks.Task<string> RequestTokenAsync(string TermID, string ResNum, long TotalAmount, long SegAmount1, long SegAmount2, long SegAmount3, long SegAmount4, long SegAmount5, long SegAmount6, string AdditionalData1, string AdditionalData2, long Wage) {
            return base.Channel.RequestTokenAsync(TermID, ResNum, TotalAmount, SegAmount1, SegAmount2, SegAmount3, SegAmount4, SegAmount5, SegAmount6, AdditionalData1, AdditionalData2, Wage);
        }
        
        public string RequestMultiSettleTypeToken(string TermID, string ResNum, string Amounts, long TotalAmount, long SegAmount1, long SegAmount2, long SegAmount3, long SegAmount4, long SegAmount5, long SegAmount6, string AdditionalData1, string AdditionalData2, long Wage) {
            return base.Channel.RequestMultiSettleTypeToken(TermID, ResNum, Amounts, TotalAmount, SegAmount1, SegAmount2, SegAmount3, SegAmount4, SegAmount5, SegAmount6, AdditionalData1, AdditionalData2, Wage);
        }
        
        public System.Threading.Tasks.Task<string> RequestMultiSettleTypeTokenAsync(string TermID, string ResNum, string Amounts, long TotalAmount, long SegAmount1, long SegAmount2, long SegAmount3, long SegAmount4, long SegAmount5, long SegAmount6, string AdditionalData1, string AdditionalData2, long Wage) {
            return base.Channel.RequestMultiSettleTypeTokenAsync(TermID, ResNum, Amounts, TotalAmount, SegAmount1, SegAmount2, SegAmount3, SegAmount4, SegAmount5, SegAmount6, AdditionalData1, AdditionalData2, Wage);
        }
    }
}
