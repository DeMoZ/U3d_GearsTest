# U3d_GearsTest
Тест был успешно завален. Фитбек от компании:

Game Gears дали ответ, что пока не готовы сделать вам предложение,основная причина - ищут кандидата с бОльшим опытом. По тесту: фидбек от лида: в архиве не вычищены лишние папки: Library, obj, .idea
вместо двух игроков 5, занятно но бажно и не соответствует заданию, поменял в настройках на 2
новая атака не происходит пока не завершится предыдущая, в исходном задании они накладываются
убитый игрок может атаковать
по коду:
на каждый удар OnHit(HitPair victim, HitPair hunter) происходит переинициализация юая _panelsController.Init(_playersController.Players) - приводит к тормозам при атаке
DamageText инстанцируется каждый раз, не используются пулы
присутствует неиспользуемый и закомменченый код
в целом код запутанный, логика не отделена от вьюх, приняты неоптимальные и критичные с точки зрения производительности решения. Спасибо за уделенное время и общение, будем на связи

Отличный фидбек, понятно куда расти.
До сего момента, за редким исключением (а все таки было раз и вполне успешно) проектировать архитектуру приложения не приходилось.
Это этого и ясно куда дальше развиваться.
