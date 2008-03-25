/*
	Copyright c 2003 by RiskCare Ltd.  All rights reserved.

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions
	are met:
	1. Redistributions of source code must retain the above copyright
	notice, this list of conditions and the following disclaimer.
	2. Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

	THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND
	ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
	IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
	ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
	FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
	DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
	OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
	HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
	LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
	OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
	SUCH DAMAGE.
*/


using System;
using SvgNet.SvgTypes;

namespace SvgNet.SvgElements
{
	/// <summary>
	/// Represents an SVG script element, which contains a script in either a text or a CDATA node.
	/// </summary>
	public class SvgScriptElement : SvgElement, IElementWithXRef, IElementWithText
	{
		public SvgScriptElement()
		{
			CDataNode tn = new CDataNode("");
			AddChild(tn);
		}

		public SvgScriptElement(string s)
		{
			CDataNode tn = new CDataNode(s);
			AddChild(tn);
		}

		public override string Name{get{return "script";}}

		public string Type
		{
			get{return (string)_atts["type"];}
			set{_atts["type"] = value;}
		}

		public SvgXRef XRef
		{
			get{return new SvgXRef(this);}
			set{value.WriteToElement(this);}
		}

		public string Href
		{
			get{return (string)_atts["xlink:href"];}
			set{_atts["xlink:href"] = value;}
		}
		
		public string Text
		{
			get{return ((CDataNode)_children[0]).Text;}
			set{((CDataNode)_children[0]).Text = value;}
		}
	}
}
