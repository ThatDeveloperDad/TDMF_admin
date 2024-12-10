using System;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.Catalog.HardCoded;
using ThatDeveloperDad.iFX.CollectionUtilities;
using ThatDeveloperDad.iFX.CollectionUtilities.Operators;

namespace TestConsole.OtherTests;

public class FilterTests
{
    public static void TestFilterClass()
    {
        Console.WriteLine("Testing the Filter class.");
        var col = StaticCatalogProvider.GetCatalog();
        
        //TestMultipleCriteria(col);

        //TestContainsOperator(col);

        TestContainedInOperator(col);

    }

    private static void TestMultipleCriteria(IEnumerable<SubscriptionTemplate> col)
    {
        Console.WriteLine("Test with simple criteria.");
        Console.WriteLine();
        Filter<SubscriptionTemplate> filter = new();
        filter.AddCriteria(
            propertyName: nameof(SubscriptionTemplate.SKU), 
            operatorKind: OperatorKinds.Equals, 
            expectedValue: StaticCatalogProvider.SKUS_TDMF_FR);
        
        

        var filtered = filter.ApplyFilter(col);

        Console.WriteLine(filter.ToString());
        Console.WriteLine();
        Console.WriteLine($"Filtered Count:  {filtered.Count()}");

        Console.WriteLine();
        Console.WriteLine("Add a criteria and do it again.");
        filter.AddCriteria(
            propertyName: nameof(SubscriptionTemplate.RenewalPeriod), 
            operatorKind: OperatorKinds.LessThan, 
            expectedValue: 100);
        
        filtered = filter.ApplyFilter(col);
        Console.WriteLine(filter.ToString());
        Console.WriteLine();
        Console.WriteLine($"Filtered Count:  {filtered.Count()}");

        Console.WriteLine();
        Console.WriteLine("-----------------");
    }

    private static void TestContainsOperator(IEnumerable<SubscriptionTemplate> col)
    {
        Console.WriteLine("Test the Contains operator.");
        Console.WriteLine();
        var filter = new Filter<SubscriptionTemplate>();
        filter.AddCriteria(
            propertyName: nameof(SubscriptionTemplate.Name), 
            operatorKind: OperatorKinds.Contains, 
            expectedValue: "Familiar");
        
        var results = filter.ApplyFilter(col);
        Console.WriteLine("If we get this far without a crash, I'm pleased.");
        Console.WriteLine(filter.ToString());
        Console.WriteLine($"Filtered Count:  {results.Count()}");

        filter.AddCriteria(
            propertyName: nameof(SubscriptionTemplate.Name), 
            operatorKind: OperatorKinds.Contains, 
            expectedValue: "Free");
        results = filter.ApplyFilter(col);
        Console.WriteLine(filter.ToString());
        Console.WriteLine($"Filtered Count:  {results.Count()}");
        Console.WriteLine();
        Console.WriteLine("-----------------");
    }

    private static void TestContainedInOperator(IEnumerable<SubscriptionTemplate> col)
    {
        Console.WriteLine("Test the ContainedIn operator.");
        Console.WriteLine();
        var filter = new Filter<SubscriptionTemplate>();
        int[] allowedSkus = new[] { 7, 30, 365 };
        filter.AddCriteria(
            propertyName: nameof(SubscriptionTemplate.RenewalPeriod), 
            operatorKind: OperatorKinds.IsContainedIn, 
            expectedValue: allowedSkus);
        
        var results = filter.ApplyFilter(col);
        Console.WriteLine("If we get this far without a crash, I'm pleased.");
        Console.WriteLine(filter.ToString());
        Console.WriteLine($"Filtered Count:  {results.Count()}");

        Console.WriteLine();
        Console.WriteLine("-----------------");
    }

}