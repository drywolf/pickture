using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace pickture.Utilities.WPF
{
    public static class INotifyPropertyChangedEx
    {
        public static void InvokeEvent(this PropertyChangedEventHandler notify_event, object sender, string property_name)
        {
            if (notify_event != null)
                notify_event(sender, new PropertyChangedEventArgs(property_name));
        }

        // source: http://stackoverflow.com/questions/527602/automatically-inotifypropertychanged/527840#527840
        public static void Notify(this PropertyChangedEventHandler EventHandler, Expression<Func<object>> Property)
        {
            // Check for null
            if (EventHandler == null)
                return;

            // Get property name
            var lambda = Property as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            ConstantExpression constantExpression;
            if (memberExpression.Expression is UnaryExpression)
            {
                var unaryExpression = memberExpression.Expression as UnaryExpression;
                constantExpression = unaryExpression.Operand as ConstantExpression;
            }
            else
            {
                constantExpression = memberExpression.Expression as ConstantExpression;
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;

            // Invoke event
            foreach (Delegate del in EventHandler.GetInvocationList())
            {
                del.DynamicInvoke(new[]
            {
                constantExpression.Value, new PropertyChangedEventArgs(propertyInfo.Name)
            });
            }
        }
    }
}
