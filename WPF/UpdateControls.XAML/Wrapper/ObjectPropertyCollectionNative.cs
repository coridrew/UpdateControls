/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections;

namespace KnockoutCS.XAML.Wrapper
{
	internal class ObjectPropertyCollectionNative : ObjectPropertyCollection
	{
        public ObjectPropertyCollectionNative(IObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
		}

        public override object TranslateOutgoingValue(object value)
        {
            return value;
        }
		public override object TranslateIncomingValue(object value)
		{
			return value;
		}
    }
}
