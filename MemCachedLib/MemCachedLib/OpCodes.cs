using System;

namespace MemCachedLib
{
	internal enum OpCodes : byte
	{
		Get,
		Set,
		Add,
		Replace,
		Delete,
		Increment,
		Decrement,
		Quit,
		Flush,
		GetQ,
		No_op,
		Version,
		GetK,
		GetKQ,
		Append,
		Prepend,
		Stat,
		SetQ,
		AddQ,
		ReplaceQ,
		DeleteQ,
		IncrementQ,
		DecrementQ,
		QuitQ,
		FlushQ,
		AppendQ,
		PrependQ,
		Verbosity,
		Touch,
		GAT,
		GATQ,
		SASL_List_Mechs = 32,
		SASL_Auth,
		SASL_Step,
		RGet = 48,
		RSet,
		RSetQ,
		RAppend,
		RAppendQ,
		RPrepend,
		RPrependQ,
		RDelete,
		RDeleteQ,
		RIncr,
		RIncrQ,
		RDecr,
		RDecrQ,
		Set_VBucket,
		Get_VBucket,
		Del_VBucket,
		TA_Connect,
		TAP_Mutation,
		TAP_Delete,
		TAP_Flush,
		TAP_Opaque,
		TAP_VBucket_Set,
		TAP_Checkpoint_Start,
		TAP_Checkpoint_End
	}
}
