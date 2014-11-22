sword
=====

WCF终结者


            using (var proxy = new Sword<ITest>())
            {
                for (var i = 0; i < 500; i++)
                {
                        var result = proxy.Proxy.Test1("test");
                }
            }
