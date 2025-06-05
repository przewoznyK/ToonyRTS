using System.Diagnostics;

internal class PlaceResourcesInStockpile : IState
{
    private readonly Gatherer _gatherer;

    public PlaceResourcesInStockpile(Gatherer gatherer)
    {
        _gatherer = gatherer;
    }

    public void Tick()
    {
   
           
    }

    public void OnEnter() {
        var returnObjectPrices = _gatherer.StockPile.AddResourcesToStockPile(_gatherer.objectPrices);
        _gatherer.SetNewObjectPricesList(returnObjectPrices);

        _gatherer._gatheredResources = 0;
    }

    public void OnExit() { }
}