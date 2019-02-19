using Armut.Common.Reacource;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Armut.Common.Result
{
    public class ServiceResult
    {
        public ServiceResult(string message = null, HttpStatusCode? resultCode = null, object data = null)
        {
            ResultCode = resultCode ?? HttpStatusCode.SeeOther;
            Data = data;
            Message = string.IsNullOrEmpty(message) ? CommonResource.OperationFailed : message;
        }
        public HttpStatusCode ResultCode { get; }
        public string Message { get; }
        public object Data { get; set; }

        public bool IsSuccess()
        {
            return ResultCode == HttpStatusCode.OK;
        }
    }
    public abstract class Result
    {
        public static ServiceResult ReturnAsSuccess(string message = null, object data = null)
        {
            return new ServiceResult(message ?? CommonResource.SuccessfulOperation, HttpStatusCode.OK, data);
        }
        public static ServiceResult ReturnAsFail(string message = null, object data = null)
        {
            return new ServiceResult(message ?? CommonResource.AnErrorOccurredWhenProcess, HttpStatusCode.BadRequest, data);
        }
        public static ServiceResult ReturnAsInformation(string message = null, object data = null)
        {
            return new ServiceResult(message ?? CommonResource.NoChanges, HttpStatusCode.SeeOther, data);
        }

        public static ServiceResult ReturnAsInformation(object productNameCanNotEmpty)
        {
            throw new NotImplementedException();
        }
    }

}
