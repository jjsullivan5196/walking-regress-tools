dat = read.csv("recog1462912905.csv", header = FALSE)
names(dat) = c("rise", "fall", "cont", "stop")

par(mfrow = c(4, 1))

plot(dat$rise, col = "red", type = "l")
abline(h = 1, col = "orange")
abline(h = 0.85, col = "orange")

plot(dat$cont, col = "green", type = "l")
abline(h = 1, col = "orange")
abline(h = 0.75, col = "orange")


plot(dat$fall, col = "blue", type = "l")
abline(h = 1, col = "orange")
abline(h = 0.65, col = "orange")


plot(dat$stop, col = "purple", type = "l")
abline(h = 2.5, col = "orange")
