using System;

namespace MemCachedLib
{
	public enum OprationStatus
	{
		No_Error,
		Key_Not_Found,
		Key_Exists,
		Value_Too_Large,
		Invalid_Arguments,
		Item_Not_Stored,
		IncrOrDecr_On_Non_Numeric_Value,
		The_Vbucket_Belongs_To_Another_Server,
		Authentication_Error,
		Authentication_Continue,
		Unknow_Command = 129,
		Out_Of_Memory,
		Not_Supported,
		Internal_Error,
		Busy,
		Temporary_Failure,
		NetworkException_HostNotAvailable
	}
}
