namespace MiniHttpServer.share;

using MiniHttpServer;
using System.Text.RegularExpressions;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

[TestClass]
public sealed class HtmlTemplateRenderer_Test
{
    [TestMethod]
    public void RenderFromString_OneProp()
    {
        var template = new HtmlTemplateRenderer();
        template.Objects.Add(
            "user",
            new User()
            {
                Name = "Berserk"
                ,
                Passport = new Passport()
                {
                    Code = 1997
                }
            }
            );

        var html = @"<p>${user.Name} - ${user.Passport.Code}</p>";
        var expected = @"<p>Berserk - 1997</p>";

        var result = template.RenderFromString(html, null);
        Assert.AreEqual( expected, result );
    }

    [TestMethod]
    public void PropRender()
    {
        var user = new User()
        {
            Name = "Andry"
            ,
            Passport = new Passport() { Code = 123 }
        };

        var template = new HtmlTemplateRenderer();
        template.Objects.Add("user", user);

        var html = @"${user.Passport.Code}";
        var result = template.RenderFromString(html, null);
        var expected = "123";

        Assert.AreEqual(expected, result, false);
    }

    [TestMethod]
    public void ForeachRender()
    {
        var data = new List<int> { 1, 2, 3 };

        var template = new HtmlTemplateRenderer();
        template.Objects.Add("data", data);

        var html =
@"$foreach(var item in data)
<p>${item} - элемент</p>
$endfor";
        
        var result = template.RenderFromString(html, null);
        var expected =
@"<p>1 - элемент</p>
<p>2 - элемент</p>
<p>3 - элемент</p>";

        Assert.AreEqual(expected, result, false);
    }

    [TestMethod]
    public void IfRender()
    {
        var template = new HtmlTemplateRenderer();
        var user = new User()
        {
            Hyligan = false,
        };
        var html =
@"Пользователь является:$if(user.Hyligan)хулиганом$elseпослушным$endif";

        template.Objects.Add("user", user);

        var result = template.RenderFromString(html, null);
        var expected =
@"Пользователь является:послушным";

        Assert.AreEqual(expected, result, false);
    }

    [TestMethod]
    public void RenderFromString_AllStatements()
    {
        var html =
@"Пользователь - ${user.Name}
является $if(user.Hyligan)
нарушителем.
$else
законопослушным гражданином.
$endif

Умеет считать до:
$foreach(var item in user.List)
${item}
$endfor";

        var template = new HtmlTemplateRenderer();

        var user = new User()
        {
            Name = "user",
            List = new List<int> {1, 2, 3, 4 },
            Hyligan = false
        };

        template.Objects.Add("user", user);

        var result = template.RenderFromString(html, null);
        var expected =
@"Пользователь - user
является законопослушным гражданином.

Умеет считать до:
1
2
3
4";
        Assert.AreEqual(expected, result, false);
    }

    [TestMethod]
    public void RenderFromString_Wrapped()
    {
        var html =
@"Список пользователей:
$foreach(var user in users)
${user.Name} - $if(user.Hyligan)
хулиган
$else
послушный
$endif
$endfor";

        var template = new HtmlTemplateRenderer();

        var users = new List<User>()
        {
            new User()
            {
                Name= "user1",
                Hyligan=false
            }
            ,new User()
            {
                Name= "user2",
                Hyligan=true
            }
        };
        template.Objects.Add("users", users);

        var result = template.RenderFromString(html, null);
        var expected =
@"Список пользователей:
user1 - послушный
user2 - хулиган";
        Assert.AreEqual(expected, result, false);
    }

    [TestMethod]
    public void RenderToString_GetObjectByReflection_NotRendered()
    {
        var user = new User()
        {
            Name = "Andry"
        };

        var template = new HtmlTemplateRenderer();
        template.Objects.Add("user", user);

        var html = @"${user.Passport.Code}";
        var result = template.RenderFromString(html, null);
        var expected = html;

        Assert.AreEqual(expected, result, false);
    }

    [TestMethod]
    public void RenderToString_ForeachRender_NotRendered()
    {
        object data = null;

        var template = new HtmlTemplateRenderer();
        template.Objects.Add("data", data);

        var html =
@"$foreach(var item in data)
<p>${item} - элемент</p>
$endfor";

        var result = template.RenderFromString(html, null);
        var expected = html;

        Assert.AreEqual(expected, result, false);
    }

    [TestMethod]
    public void RenderToString_IfRender_NotRendered()
    {
        var template = new HtmlTemplateRenderer();
        var user = new User()
        {
            Hyligan = false,
        };

        // Прописываем не сущесвтующее свойство
        var html =
@"Пользователь является:
$if(user.Hyliganist)
хулиганом
$else
послушным
$endif";

        template.Objects.Add("user", user);

        var result = template.RenderFromString(html, null);
        var expected = html;

        Assert.AreEqual(expected, result, false);
    }

    [TestMethod]
    public void RenderFromFile()
    {
        var h = new HtmlTemplateRenderer();
        var str = h.RenderFromFile("./Static/Index2.html", null);
    }
}
