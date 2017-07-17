using Kendo.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Accent.Security.Business.Global
{
    public static class KendoFilterUtil
    {

        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains");
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        public static TEntity GetFilterMapper<TEntity>(IList<IFilterDescriptor> gridFilterDescriptors, TEntity model) where TEntity : class
        {

            if (gridFilterDescriptors != null && gridFilterDescriptors.Count > 0)
            {
                Type filterItemType = gridFilterDescriptors[0].GetType();
                if (filterItemType.Equals(typeof(FilterDescriptor)))
                {
                    BuildSimpleFilterDescriptors_Kendo(gridFilterDescriptors, model);
                }
                else if (filterItemType.Equals(typeof(CompositeFilterDescriptor))) //SP: If it is Composite Filter
                {
                    BuildCompositeFilterDescriptors(gridFilterDescriptors, model);

                }
            }
            return model;
        }


        public static Expression<Func<T, bool>> GetFilterExpression<T>(IList<IFilterDescriptor> gridFilterDescriptors)
        {
            if (gridFilterDescriptors.Count == 0)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (gridFilterDescriptors.Count > 0)
            {

                Type filterItemType = gridFilterDescriptors[0].GetType();
                if (filterItemType.Equals(typeof(FilterDescriptor)))
                {
                    foreach (FilterDescriptor filterItem in gridFilterDescriptors)
                    {
                        if (exp == null)
                            exp = GetExpression<T>(filterItem.Operator.ToString(), param, filterItem);
                        else
                            exp = Expression.AndAlso(exp, GetExpression<T>(filterItem.Operator.ToString(), param, filterItem));

                    }
                }
                else if (filterItemType.Equals(typeof(CompositeFilterDescriptor))) //SP: If it is Composite Filter
                {
                    foreach (CompositeFilterDescriptor comfilterItem in gridFilterDescriptors)
                    {
                        foreach (var filterItem in comfilterItem.FilterDescriptors)
                        {
                            var filterDesc = (FilterDescriptor)filterItem;
                            if (exp == null)
                                exp = GetExpression<T>(filterDesc.Operator.ToString(), param, filterDesc);
                            else
                                exp = Expression.AndAlso(exp, GetExpression<T>(filterDesc.Operator.ToString(), param, filterDesc));

                        }
                    }
                }
            }



            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        private static Expression GetExpression<T>(string functionName, ParameterExpression param, FilterDescriptor filterDescriptor)
        {
            MemberExpression member = Expression.Property(param, filterDescriptor.Member);
            ConstantExpression constant = Expression.Constant(filterDescriptor.Value);
            switch (filterDescriptor.Operator.ToString())
            {
                case "IsEqualTo":
                    return Expression.Equal(member, constant);

                case "IsNotEqualTo":
                    return Expression.NotEqual(member, constant);

                case "IsGreaterThan":
                    return Expression.GreaterThan(member, constant);

                case "IsLessThan":
                    return Expression.LessThan(member, constant);

                case "IsGreaterThanOrEqualTo":
                    return Expression.GreaterThanOrEqual(member, constant);

                case "IsLessThanOrEqualTo":
                    return Expression.LessThanOrEqual(member, constant);

                case "StartsWith":
                    return Expression.Call(member, startsWithMethod, constant);

                case "EndsWith":
                    return Expression.Call(member, endsWithMethod, constant);

                case "IsNull":
                    return Expression.Equal(member, null);

                case "NotIsNull":
                    return Expression.NotEqual(member, null);

                case "Contains":
                    return Expression.Call(member, containsMethod, constant);

            }
            return null;
        }

        private static void BuildSimpleFilterDescriptor_Kendo(FilterDescriptor simpleFilter, object model)
        {
            PropertyInfo prop = model.GetType().GetProperty(simpleFilter.Member);
            prop.SetValue(model, simpleFilter.Value, null);
        }

        private static void BuildSimpleFilterDescriptors_Kendo(IList<IFilterDescriptor> filterDescriptors, object model)
        {
            //SP: Traverse each filteritem and process the filter item
            foreach (FilterDescriptor filterItem in filterDescriptors)
            {
                PropertyInfo prop = model.GetType().GetProperty(filterItem.Member);
                prop.SetValue(model, filterItem.Value, null);
            }
        }

        private static void BuildCompositeFilterDescriptors(IList<IFilterDescriptor> compositeFilterDescriptors, object model)
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
                    BuildSimpleFilterDescriptor_Kendo(simpleFilter, model);
                }
                else if (filterType.Equals(typeof(CompositeFilterDescriptor))) //SP: if it is composite descriptor
                {
                    var compositeFilter = ((CompositeFilterDescriptor)(compositeFilterDescriptors[descriptorCount]));
                    BuildCompositeFilterDescriptor(compositeFilter, model); ;
                }
            }
        }

        private static void BuildCompositeFilterDescriptor(CompositeFilterDescriptor compositeFilterDescriptor, object model)
        {
            //SP: Traverse each filter item and process it
            for (
                int filterDescriptorCount = 0,
                    filterDescriptorLength = compositeFilterDescriptor.FilterDescriptors.Count;
                filterDescriptorCount < filterDescriptorLength; ++filterDescriptorCount)
            {
                //SP: Finding the filter descriptor type
                Type itemType = compositeFilterDescriptor.FilterDescriptors[filterDescriptorCount].GetType();
                if (itemType.Equals(typeof(FilterDescriptor))) //SP: if it is simple filter
                {
                    var filterDesc = ((FilterDescriptor)(compositeFilterDescriptor.FilterDescriptors[filterDescriptorCount]));
                    PropertyInfo prop = model.GetType().GetProperty(filterDesc.Member);
                    prop.SetValue(model, filterDesc.Value, null);
                }
                else if (itemType.Equals(typeof(CompositeFilterDescriptor))) //SP: if it is composite filter
                {
                    var compositeFilter = ((CompositeFilterDescriptor)(compositeFilterDescriptor.FilterDescriptors[filterDescriptorCount]));
                    BuildCompositeFilterDescriptor(compositeFilter, model);
                }
            }
        }
    }

}
