sword
=====

WCF终结者
SwordConfiguration.SetServerInfo("localhost", 888);

            using (var proxy = new Sword<ITest>())
            {
                for (var i = 0; i < 500; i++)
                {
                    try
                    {
                        var result = proxy.Proxy.Test1("fff");

                        Console.WriteLine(i + "====" + result.P1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
