/*----------------------------------------------------------------
* 项目名称 ：WebApi.Utils
* 类 名 称 ：MultiXmlDocumentationProvider
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi.Utils
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:54:00
* 更新时间 ：2018/10/15 14:54:00
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using WebApi.Areas.HelpPage;
using WebApi.Areas.HelpPage.ModelDescriptions;

namespace WebApi.Utils
{
    /// <summary>A custom <see cref='IDocumentationProvider'/> that reads the API documentation from a collection of XML documentation files.</summary>  
    public class MultiXmlDocumentationProvider : IDocumentationProvider, IModelDocumentationProvider
    {
        /********* 
        ** Properties 
        *********/
        /// <summary>The internal documentation providers for specific files.</summary>  
        private readonly XmlDocumentationProvider[] Providers;


        /********* 
        ** Public methods 
        *********/
        /// <summary>Construct an instance.</summary>  
        /// <param name='paths'>The physical paths to the XML documents.</param>  
        public MultiXmlDocumentationProvider(params string[] paths)
        {
            this.Providers = paths.Select(p => new XmlDocumentationProvider(p)).ToArray();
        }

        /// <summary>Gets the documentation for a subject.</summary>  
        /// <param name='subject'>The subject to document.</param>  
        public string GetDocumentation(MemberInfo subject)
        {
            return this.GetFirstMatch(p => p.GetDocumentation(subject));
        }

        /// <summary>Gets the documentation for a subject.</summary>  
        /// <param name='subject'>The subject to document.</param>  
        public string GetDocumentation(Type subject)
        {
            return this.GetFirstMatch(p => p.GetDocumentation(subject));
        }

        /// <summary>Gets the documentation for a subject.</summary>  
        /// <param name='subject'>The subject to document.</param>  
        public string GetDocumentation(HttpControllerDescriptor subject)
        {
            return this.GetFirstMatch(p => p.GetDocumentation(subject));
        }

        /// <summary>Gets the documentation for a subject.</summary>  
        /// <param name='subject'>The subject to document.</param>  
        public string GetDocumentation(HttpActionDescriptor subject)
        {
            return this.GetFirstMatch(p => p.GetDocumentation(subject));
        }

        /// <summary>Gets the documentation for a subject.</summary>  
        /// <param name='subject'>The subject to document.</param>  
        public string GetDocumentation(HttpParameterDescriptor subject)
        {
            return this.GetFirstMatch(p => p.GetDocumentation(subject));
        }

        /// <summary>Gets the documentation for a subject.</summary>  
        /// <param name='subject'>The subject to document.</param>  
        public string GetResponseDocumentation(HttpActionDescriptor subject)
        {
            return this.GetFirstMatch(p => p.GetDocumentation(subject));
        }


        /********* 
        ** Private methods 
        *********/
        /// <summary>Get the first valid result from the collection of XML documentation providers.</summary>  
        /// <param name='expr'>The method to invoke.</param>  
        private string GetFirstMatch(Func<XmlDocumentationProvider, string> expr)
        {
            return this.Providers
                .Select(expr)
                .FirstOrDefault(p => !String.IsNullOrWhiteSpace(p));
        }
    }
}