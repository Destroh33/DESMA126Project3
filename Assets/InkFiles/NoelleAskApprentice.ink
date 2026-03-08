EXTERNAL AcceptNoelle()

Noelle: You've shown me a bunch of different fish already, how many are there in the sea? I want to look at them all! #portrait:noelle
Sam: Well, I don't think even us fishermen know for sure. New ones pop up every few years and some types of fish also go away. #portrait:sam
Noelle: Would you maybe teach me how to fish so I can see them all then, Sam? #portrait:noelle

*[Sure!] -> accept_noelle
*[I need time to think...] -> time_to_think

==accept_noelle==
Sam: Sure thing! I'd be happy to show you the ropes. Come meet me at the docks after school tomorrow, and we'll get started! #portrait:sam
Noelle: Do I need to have my own rod? I don't have any money to buy one... #portrait:noelle
Sam: Don't worry, I've always got spares. #portrait:sam
Noelle: Yay! I'll be here tomorrow, then! #portrait:noelle
~ AcceptNoelle()
-> END

==time_to_think==
Sam: I'm not sure... let me think on it for a bit. #portrait:sam
Noelle: Okay! I'll be around if you want to take me as a student, just talk to me! #portrait:noelle

-> END
