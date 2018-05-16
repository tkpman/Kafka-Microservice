// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.7.7.4
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Order.Api.Application.Events
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	public partial class ReservedOrder : ISpecificRecord
	{
		public static Schema _SCHEMA = Schema.Parse("{\"type\":\"record\",\"name\":\"ReservedOrder\",\"namespace\":\"Order.Api.Application.Events" +
				"\",\"fields\":[{\"name\":\"OrderId\",\"type\":\"string\"},{\"name\":\"Status\",\"type\":\"string\"}" +
				",{\"name\":\"Total\",\"type\":\"double\"}]}");
		private string _OrderId;
		private string _Status;
		private double _Total;
		public virtual Schema Schema
		{
			get
			{
				return ReservedOrder._SCHEMA;
			}
		}
		public string OrderId
		{
			get
			{
				return this._OrderId;
			}
			set
			{
				this._OrderId = value;
			}
		}
		public string Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		public double Total
		{
			get
			{
				return this._Total;
			}
			set
			{
				this._Total = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.OrderId;
			case 1: return this.Status;
			case 2: return this.Total;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.OrderId = (System.String)fieldValue; break;
			case 1: this.Status = (System.String)fieldValue; break;
			case 2: this.Total = (System.Double)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}