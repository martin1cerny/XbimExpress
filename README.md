# EXPRESS code generator

This repository contains utilities to generate C# code from EXPRESS schema definitions.
Resulting code depends on [Xbim.Common](https://github.com/xBimTeam/XbimEssentials/tree/master/Xbim.Common) 
which contains common interfaces and abstract classes designed for work with generated schema implementations. 

This project contains an implementation of part of [SDAI](https://en.wikipedia.org/wiki/ISO_10303-22)
which describes structure of EXPRESS language. Parser generated with [GPPG](https://gppg.codeplex.com/)/[GPLEX](https://gplex.codeplex.com/)
is used to load EXPRESS schema into object model. This is a separate project and can be used on its own 
to perform other tasks based on the instantiated EXPRESS schema. We use this only to feed 
[T4](https://en.wikipedia.org/wiki/Text_Template_Transformation_Toolkit) templates to generate C# code.
But it could equally be used for late bingind applications as well as various analyses of the schema.

Generator is not able to process any arbitrary EXPRESS definition but was implemented to
the level where it can generate implementation of IFC schemas. It can generate any schema which 
uses the same constrains and design patterns. EXPRESS language allows for multiple instance inheritance
for example which doesn't suit our implementation patterns. 

Also validation rules are not implemented. Saying so, it is poossible to use the generator
to create stubs for implementation of validation routines. Simple derived attributes are supported
but for more complicated cases only a stub is generated with *TODO* comment and `NotImplementedException`.
So, make sure you check all *TODO* tasks after the generation. 

White generated code will always override existing files, there are special blocs generated in the code
which will survive regeneration. At the same time, all classes are generated as *partial*, so it is possible
to define additional functionality in partial files sitting somewhere else in the project.

To access the data using generated schema you can either implement your own `IModel` and related interfaces
or you can use [Xbim.IO](https://github.com/xBimTeam/XbimEssentials/tree/master/Xbim.IO) which can load STEP21
physical representation of the schema into the generated implementation of the schema. You can than use specific
implementation of `IModel` to access, create of change the data. It is also possible to save and load data in XML
but that uses configuration and settings specific to IFC and is different for IFC2x3 and IFC4.