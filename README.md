# SenseNet-Ubooquity-Gateway

Just checking possibilities to connect sensenet repository or use it's api as a gateway to an Ubooquity library through opds feed.

## Possibilities

- make direct contact to Ubooquity with handler and routing
- use sensenet application as in repository handler
- use odata action as dynamic handler
- use hardlinked contents as placeholders

## Handler with routing

This solution get around sensenet and routes request to ubooquity api through a handler.

It uses:
- an http handler 
- a routehandler
- modified global.asax with registered routes

## sensenet application

This solution is yet to think through. Possibly it will work as
- has to have a root Application to serve opds start point
- navigation link has to be altered to match sensenet action link handling

## Odata action

This solution relies on client to use navigation wisely. 

It uses:
- only an odata action

## hardlinked contents

This solution needs to be worked out as well. Probably it can provide additional functionality to above solutions, rather than a self-sufficing method.
Works as external data representation it's a placeholder Content directly pointing at a certain feed. this could be a selected folder or a book in Ubooquity library and store the response xml in repository.