# Randolph 1.1 Chess Master by Josh Dotson 2017

# Acknowledgements
This chess engine was inspired by the teachings of the late chessprogrammingwiki, and the brillant Bluefever Software. Bluefever offered an AMAZING tutorial series regarding programming a chess engine in C, and although Randolph uses C# and .NET, his videos had a large impact on the structure my engine and taught me many algorithmic concepts that underly chess engines. The chessprogrammingwiki, now defunked, offered massive compilation of knowledge regarding all known concepts of engine theory as well as their implementations. 

# Summary
Named after Randolph Carter, my favorite character from HP Lovecraft's Dream Cycle, as well as the protagonist of one of my favorite stories "The Dream Quest of Unknown Kadath", Randolph 1.1 is a stateless chess engine using UCI for integration with any GUI front end; such as Arena. It also fully implements the parsing and plotting of Forsyth-Edwards notated strings. 

Randolph starts by analyzing a position, and evaluating it in terms of "centi-pawns". He does this evaluation recursivley using his Alpha-Beta MINMAX searching algorithm millions of times a second. This also allows Randolph to search to a specified depth, or decide his own depth based on his time left of the clock. Once he has decided there isn't enough time to think deeper into the position, he will grab the best line out of his princple variation table, fall back to the root node of his search tree, and make his move. 

I have never successfully beat Randolph myself, and based on the games I've witnessed him play using a i7 2630QM @ 2.6 GHz, his strength is somewhere in the ballpark of 1900-2100 ELO/USCF. It was a blast making Randolph and it allowed me to have an even deeper appreciation for the mathmatical beauty behind Chess.
