namespace ParserQueueRunner.Model
{
    interface IWebPageParser
    {
        WebParserResult ParseWebSite(WebParserConfig config, WebParserParams param);
        WebParserResult ParserCheck(WebParserConfig config);
        string GetParserPath();
    }
}
