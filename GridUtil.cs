#region Copyright

// <copyright  project="Wellness Client Access" company="SEB Wellness">Copyright (c) 2014.All rights reserved</copyright>
// <summary></summary>
// <author>Kiran Kumar Banda</author>
// <createdon>2014-06-24 3:25 PM</createdon>
// <modifiedon></modifiedon>
// <modifiedby></modifiedby>
// <changelog>
// 
// </changelog>

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Kendo.Mvc;

namespace Wellness.Common
{
    /// <summary>
    /// Grid Util
    /// </summary>
    public class GridUtil
    {
        #region Variables

        /// <summary>
        /// The where clause XML
        /// </summary>
        private StringBuilder _whereClauseXml;

        #endregion

        #region Enumerators

        /// <summary>
        /// Grid Filter Operator
        /// </summary>
        private enum GridFilterOperator
        {
            NoFilter = -1,
            IsNull = 0,
            NotIsNull = 1,
            EqualTo = 2,
            NotEqualTo = 3,
            StartsWith = 4,
            EndsWith = 5,
            Contains = 6,
            GreaterThan = 7,
            GreaterThanOrEqualTo = 8,
            LessThan = 9,
            LessThanOrEqualTo = 10
        }

        #endregion

        #region Constructors

        #endregion

        #region Kendo Helpers

        /// <summary>
        /// Gets the where clause XML_ kendo.
        /// </summary>
        /// <param name="gridFilterDescriptors">The grid filter descriptors.</param>
        /// <returns></returns>
        public string GetWhereClauseXml_Kendo(IList<IFilterDescriptor> gridFilterDescriptors)
        {
            _whereClauseXml = new StringBuilder();
            if (gridFilterDescriptors != null)
            {
                var settings = new XmlWriterSettings { OmitXmlDeclaration = true };

                using (XmlWriter writer = XmlWriter.Create(_whereClauseXml, settings))
                {
                    if (gridFilterDescriptors.Count > 0)
                    {
                        writer.WriteStartElement("ROW");
                        writer.WriteStartElement("Filter");

                        Type filterItemType = gridFilterDescriptors[0].GetType();
                        //SP: If it is Simple Filter
                        if (filterItemType.Equals(typeof(FilterDescriptor)))
                        {
                            BuildSimpleFilterDescriptors_Kendo(gridFilterDescriptors, writer);
                        }
                        else if (filterItemType.Equals(typeof(CompositeFilterDescriptor))) //SP: If it is Composite Filter
                        {
                            BuildCompositeFilterDescriptors_Kendo(gridFilterDescriptors, writer);
                        }

                        writer.WriteEndElement(); // End Filter
                        writer.WriteEndElement(); // End Row
                        writer.Close();
                    }
                }
            }
            var result = _whereClauseXml.ToString();
            if (result.Trim().Length == 0) { return null; }
            return _whereClauseXml.ToString();
        }


        /// <summary>
        /// Builds the simple filter descriptors_ kendo.
        /// </summary>
        /// <param name="filterDescriptors">The filter descriptors.</param>
        /// <param name="writer">The writer.</param>
        private void BuildSimpleFilterDescriptors_Kendo(IList<IFilterDescriptor> filterDescriptors, XmlWriter writer)
        {
            //SP: Traverse each filteritem and process the filter item
            foreach (FilterDescriptor filterItem in filterDescriptors)
            {
                AppendFilterToWhereClause_Kendo(filterItem, writer);
            }
        }

        public string GetWhereClauseXml_Composite(IList<IFilterDescriptor> gridFilterDescriptors)
        {
            _whereClauseXml = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };

            using (XmlWriter writer = XmlWriter.Create(_whereClauseXml, settings))
            {
                if (gridFilterDescriptors.Count > 0)
                {
                    writer.WriteStartElement("ROW");
                    writer.WriteStartElement("Filter");

                    Type filterItemType = gridFilterDescriptors[0].GetType();
                    //SP: If it is Simple Filter
                    if (filterItemType.Equals(typeof(FilterDescriptor)))
                    {
                        BuildSimpleFilterDescriptors_Kendo(gridFilterDescriptors, writer);
                    }
                    else if (filterItemType.Equals(typeof(CompositeFilterDescriptor))) //SP: If it is Composite Filter
                    {
                        //BuildCompositeFilterDescriptors_CompositeDate(gridFilterDescriptors, writer);
                    }

                    writer.WriteEndElement(); // End Filter
                    writer.WriteEndElement(); // End Row
                    writer.Close();
                }
            }
            var result = _whereClauseXml.ToString();
            if (result.Trim().Length == 0) { return null; }
            return _whereClauseXml.ToString();
        }



        /// <summary>
        /// Gets the conditional operator.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        private GridFilterOperator GetConditionalOperator(string functionName)
        {
            var conditionalOperator = GridFilterOperator.NoFilter;
            switch (functionName)
            {
                case "IsEqualTo":
                    conditionalOperator = GridFilterOperator.EqualTo;
                    break;
                case "IsNotEqualTo":
                    conditionalOperator = GridFilterOperator.NotEqualTo;
                    break;
                case "IsGreaterThan":
                    conditionalOperator = GridFilterOperator.GreaterThan;
                    break;
                case "IsLessThan":
                    conditionalOperator = GridFilterOperator.LessThan;
                    break;
                case "IsGreaterThanOrEqualTo":
                    conditionalOperator = GridFilterOperator.GreaterThanOrEqualTo;
                    break;
                case "IsLessThanOrEqualTo":
                    conditionalOperator = GridFilterOperator.LessThanOrEqualTo;
                    break;
                case "StartsWith":
                    conditionalOperator = GridFilterOperator.StartsWith;
                    break;
                case "EndsWith":
                    conditionalOperator = GridFilterOperator.EndsWith;
                    break;
                case "IsNull":
                    conditionalOperator = GridFilterOperator.IsNull;
                    break;
                case "NotIsNull":
                    conditionalOperator = GridFilterOperator.NotIsNull;
                    break;
                case "Contains":
                    conditionalOperator = GridFilterOperator.Contains;
                    break;
                default:
                    conditionalOperator = GridFilterOperator.Contains;
                    break;
            }
            return conditionalOperator;
        }


        /// <summary>
        /// Appends the filter to where clause_ kendo.
        /// </summary>
        /// <param name="filterItem">The filter item.</param>
        /// <param name="writer">The writer.</param>
        private void AppendFilterToWhereClause_Kendo(FilterDescriptor filterItem, XmlWriter writer)
        {
            //SP: Append required members (Operator, value, etc.,) from the filterItem to WhereClauseXml
            GridFilterOperator filterOperator = GetConditionalOperator(filterItem.Operator.ToString());
            string filterValue = filterItem.Value.ToString(); //((filterItem.Value.ToString().Replace("&", "")).Replace("<", "")).Replace("\"", "");

            writer.WriteAttributeString(string.Format("{0}Operator", filterItem.Member),
                ((int)filterOperator).ToString());
            writer.WriteAttributeString(string.Format("{0}Value", filterItem.Member), filterValue);
        }


        /// <summary>
        /// Builds the composite filter descriptors_ kendo.
        /// </summary>
        /// <param name="compositeFilterDescriptors">The composite filter descriptors.</param>
        /// <param name="writer">The writer.</param>
        private void BuildCompositeFilterDescriptors_Kendo(IList<IFilterDescriptor> compositeFilterDescriptors,
            XmlWriter writer)
        {
            //SP: Traverse each filter descriptior, find whether it is simple or composite and process it
            for (int descriptorCount = 0, descriptorLength = compositeFilterDescriptors.Count;
                descriptorCount < descriptorLength;
                ++descriptorCount)
            {
                string filterMember = string.Empty;
                Type filterType = compositeFilterDescriptors[descriptorCount].GetType();
                if (filterType.Equals(typeof(FilterDescriptor))) //SP: if it is simple descriptor
                {
                    var simpleFilter = ((FilterDescriptor)(compositeFilterDescriptors[descriptorCount]));
                    filterMember = BuildSimpleFilterDescriptor_Kendo(simpleFilter, filterMember, writer);
                }
                else if (filterType.Equals(typeof(CompositeFilterDescriptor))) //SP: if it is composite descriptor
                {
                    var compositeFilter = ((CompositeFilterDescriptor)(compositeFilterDescriptors[descriptorCount]));
                    filterMember = BuildCompositeFilterDescriptor_Kendo(compositeFilter, filterMember, writer);
                }
            }
        }


        /// <summary>
        /// Builds the simple filter descriptor_ kendo.
        /// </summary>
        /// <param name="simpleFilter">The simple filter.</param>
        /// <param name="filterMember">The filter member.</param>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        private string BuildSimpleFilterDescriptor_Kendo(FilterDescriptor simpleFilter, string filterMember,
            XmlWriter writer)
        {
            if (string.IsNullOrEmpty(filterMember) || !filterMember.Equals(simpleFilter.Member))
            {
                AppendFilterToWhereClause_Kendo(simpleFilter, writer);
            }
            return simpleFilter.Member;
        }


        /// <summary>
        /// Builds the composite filter descriptor_ kendo.
        /// </summary>
        /// <param name="compositeFilterDescriptor">The composite filter descriptor.</param>
        /// <param name="filterMember">The filter member.</param>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        private string BuildCompositeFilterDescriptor_Kendo(CompositeFilterDescriptor compositeFilterDescriptor,
            string filterMember, XmlWriter writer)
        {
            //SP: Traverse each filter item and process it
            for (
                int filterDescriptorCount = 0,
                    filterDescriptorLength = compositeFilterDescriptor.FilterDescriptors.Count;
                filterDescriptorCount < filterDescriptorLength;
                ++filterDescriptorCount)
            {
                //SP: Finding the filter descriptor type
                Type itemType = compositeFilterDescriptor.FilterDescriptors[filterDescriptorCount].GetType();
                if (itemType.Equals(typeof(FilterDescriptor))) //SP: if it is simple filter
                {
                    var simpleFilter =
                        ((FilterDescriptor)(compositeFilterDescriptor.FilterDescriptors[filterDescriptorCount]));
                    filterMember = BuildSimpleFilterDescriptor_Kendo(simpleFilter, filterMember, writer);
                }
                else if (itemType.Equals(typeof(CompositeFilterDescriptor))) //SP: if it is composite filter
                {
                    var compositeFilter =
                        ((CompositeFilterDescriptor)
                            (compositeFilterDescriptor.FilterDescriptors[filterDescriptorCount]));
                    filterMember = BuildCompositeFilterDescriptor_Kendo(compositeFilter, filterMember, writer);
                }
            }

            return filterMember;
        }

       
        #endregion
    }
}