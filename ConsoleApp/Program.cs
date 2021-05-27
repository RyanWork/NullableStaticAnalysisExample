using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualBasic;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }

        #region AllowNull

        private static void AllowNullExample()
        {
            AllowNullClass allowNullClass = new()
            {
                SomeValue = null
            };
        }
        
        private class AllowNullClass
        {
            /**
             * AllowNull tells the compiler that we know a particular property/field
             * can be set to null and we allow it because we handle it. We know that this
             * field will never be null, but we don't restrict the caller from setting it
             * to null. This allows us to not explicitly set it to string? where all
             * callers from then on are suggested for checking that a property/field is null.
             */
            // [AllowNull]
            public string SomeValue 
            { 
                get => _someValue; 
                set => _someValue = value ?? string.Empty; 
            }

            private string _someValue = string.Empty;
        }

        #endregion

        #region DisallowNull

        private static void DisallowNullExample()
        {
            DisallowNullClass disallowNullClass = new()
            {
                SomeValue = null
            };
        }
        
        private class DisallowNullClass
        {
            /**
             * DisallowNull generates compiler warnings implying that a value cannot be set to null.
             * Despite this value being nullable (which is true for when the object is first created),
             * our intent is that a caller can never set this value explicitly to null. In this example,
             * setting a value to null will throw an exception.
             */
            // [DisallowNull]
            public string? SomeValue
            {
                get => _someValue;
                set => _someValue = value ?? throw new ArgumentNullException(nameof(value), "Cannot set to null");
            }

            private string? _someValue;
        }

        #endregion
        
        #region MaybeNull
        
        /**
         * MaybeNull specifies notifies the compiler that something could null.
         * Pretty self explanatory, this apparently has some use-cases specifically for
         * generic types where it's inferred that a generic can be both a value-type or reference-type
         * in which it is impossible to return a null object. Consider a scenario where we are able
         * to return both objects and value types where objects can be null but an int cannot.
         * Unfortunately, I can't actually reproduce that scenario so this example kind of sucks. Sorry!
         */
        [return: MaybeNull]
        public static T Generic<T>(T param) where T : class
        {
            return null;
        }

        /**
         * You may be wondering how this is different.
         * T? is syntactic sugar for Nullable<T> which is a form of boxing.
         * In theory, this might save you the operations to box/unbox an actual object.
         * Dont quote me on that.
         */
        public static T? Generic2<T>(T param) where T : class
        {
            return null;
        }
        
        // sharplab.io example:
        // https://sharplab.io/#v2:CYLg1APgAgTAjAWAFBQAwAIpwCwG5lqZwB0AIgJYCGA5gHYD2AzgC7kDGjxAwvcAKYBBWpQA2AT0blG+JAQDMmGOi7IA3snSatmgNpQA7CHQBZSmIBGfAHIBXESIC6G7VqgKsANnQAVdAHE+Wj4AJ3YAHm8APgAKXwAHSmDKAFsASnQAdwALEL4fdCNYZxd0dSQSkoN0WjsRGQr0AF9i9BbtNyIvbwB+f0CQ9hgImPjElPTs3PzCmDatMob2/Wra+ormpEagA===
        
        #endregion

        #region NotNull

        private static void NotNullExample(string? someString)
        {
            ThrowIfNull(someString);
            Console.WriteLine(someString.Length);
        }

        /**
         * This chains warnings from callers where we can implicitly check within a nested call
         * that a parameter that is passed in already performs a null check. Subsequent checks for
         * nullability in other methods would be redundant and make code less readable.
         * Adding the NotNull attribute will prevent the compiler from generating warnings to
         * additional calls when a check is already performed.
         */
        private static void ThrowIfNull(
            // [NotNull] 
            object? someObject)
        {
            if (someObject is null)
                throw new ArgumentNullException(nameof(someObject), "Cannot set to null");
        }

        #endregion

        #region MemberNotNull
        
        private class MemberNotNullClass
        {
            public string UninitializedProperty { get; set; }

            public MemberNotNullClass()
            {
                Initialize();
            }
                
            /**
             * MemberNotNull prevents the compiler generated warnings
             * for properties/fields that are initialized outside of the constructor.
             * But we can guarantee that the field is initialized through a helper method.
             */
            // [MemberNotNull(nameof(UninitializedProperty))]
            private void Initialize()
            {
                this.UninitializedProperty = string.Empty;
            }
        }
        
        #endregion
    }
}